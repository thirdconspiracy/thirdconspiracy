const pad = (n: number) => String(n).padStart(2, '0');

export type DurationFormat = 's' | 'mm:ss' | 'hh:mm:ss';

// Which display format 'formatDuration' uses for the given seconds
export function durationFormat(totalSeconds: number): DurationFormat {
  const total = Math.max(0, Math.floor(totalSeconds));
  if (total < 60) return 's';
  if (total < 3600) return 'mm:ss';
  return 'hh:mm:ss';
}

// Formats seconds as S (<1m), MM:SS (<1h), or HH:MM:SS
export function formatDuration(totalSeconds: number): string {
  const total = Math.max(0, Math.floor(totalSeconds));
  const h = Math.floor(total / 3600);
  const m = Math.floor((total % 3600) / 60);
  const s = total % 60;

  switch (durationFormat(total)) {
    case 's':
      return String(s);
    case 'mm:ss':
      return `${pad(m)}:${pad(s)}`;
    case 'hh:mm:ss':
      return `${pad(h)}:${pad(m)}:${pad(s)}`;
  }
}
