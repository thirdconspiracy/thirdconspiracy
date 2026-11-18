import { Injectable } from '@angular/core';

export interface FocusTrack {
  id: number;
  title: string;
}

const DB_NAME= 'focus-zone';
const DB_VERSION= 1;
/** Track metadata ({ id, title }), kept apart from the audio so listening stays cheap. */
const TRACKS = 'tracks';
/** Audio bytes ({ type, data }) keyed by track id. */
const AUDIO = 'audio';

interface StoredAudio {
  type: string;
  data: ArrayBuffer;
}

const result = <T>(request: IDBRequest<T>): Promise<T> =>
  new Promise((resolve, reject) => {
    request.onsuccess = () => resolve(request.result);
    request.onerror = () => reject(request.error);
  });

const completion = (tx: IDBTransaction): Promise<void> =>
  new Promise((resolve, reject) => {
    tx.oncomplete = () => resolve();
    tx.onerror = tx.onabort = () => reject(tx.error ?? new DOMException('Transaction aborted', 'AbortError'));
  });

/**
 * Music the user added, saved in this browser's IndexedDB. Files never leave the
 * device, so licensed copies aren't redistributed.
 */
@Injectable({ providedIn: 'root' })
export class FocusTrackStore {
  private db?: Promise<IDBDatabase>;

  async list(): Promise<FocusTrack[]> {
    const db = await this.open();
    return result(db.transaction(TRACKS).objectStore(TRACKS).getAll());
  }

  async add(title: string, file: Blob): Promise<FocusTrack> {
    const audio: StoredAudio = { type: file.type || 'audio/mpeg', data: await file.arrayBuffer() };
    const db = await this.open();
    const tx = db.transaction([TRACKS, AUDIO], 'readwrite');
    const request = tx.objectStore(TRACKS).add({ title });
    request.onsuccess = () => tx.objectStore(AUDIO).add(audio, request.result);
    try {
      await completion(tx);
    } catch (e) {
      throw (e as DOMException).name === 'QuotaExceededError'
        ? new Error('Not enough browser storage for this song.')
        : e;
    }
    return { id: request.result as number, title };
  }

  async audio(id: number): Promise<Blob> {
    const db = await this.open();
    const stored = await result<StoredAudio | undefined>(db.transaction(AUDIO).objectStore(AUDIO).get(id));
    if (!stored) throw new Error('Song data is missing.');
    return new Blob([stored.data], { type: stored.type });
  }

  async remove(id: number): Promise<void> {
    const db = await this.open();
    const tx = db.transaction([TRACKS, AUDIO], 'readwrite');
    tx.objectStore(TRACKS).delete(id);
    tx.objectStore(AUDIO).delete(id);
    await completion(tx);
  }

  private open(): Promise<IDBDatabase> {
    this.db ??= new Promise<IDBDatabase>((resolve, reject) => {
      const request = indexedDB.open(DB_NAME, DB_VERSION);
      request.onupgradeneeded = () => {
        request.result.createObjectStore(TRACKS, { keyPath: 'id', autoIncrement: true});
        request.result.createObjectStore(AUDIO);
      };
      request.onsuccess = () => resolve(request.result);
      request.onerror = () => reject(request.error);
    }).catch((e: unknown) => {
      this.db = undefined;
      throw e;
    });
    return this.db;
  }
}
