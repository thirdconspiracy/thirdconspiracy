/** Reads/writes small JSON settings objects in localStorage, ignoring bad data */
export function loadSettings<T extends object>(key: string, defaults: T): T {
  try {
    const stored: unknown = JSON.parse(localStorage.getItem(key) ?? 'null');
    return stored && typeof stored === 'object' ? { ...defaults, ...(stored as Partial<T>) } : defaults;
  } catch {
    return defaults;
  }
}

export function saveSettings(key: string, value: object): void {
  localStorage.setItem(key, JSON.stringify(value));
}

/** Clamps a volume to 0..1 (non-numbers become the fallback) */
export const clampVolume = (value: unknown, fallback = 0.5): number =>
  typeof value === 'number' && Number.isFinite(value) ? Math.min(1, Math.max(0, value)) : fallback;

/** Maps a 0..1 slider position to a gain so equal slider steps sound like equal loudness steps */
export const sliderGain = (volume: number): number => volume * volume;
