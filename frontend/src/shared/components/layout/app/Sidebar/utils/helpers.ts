/**
 * Generate user initials from full name
 * @param name - Full name
 * @param fallback - Fallback character if name is empty
 * @returns Initials (up to 2 characters)
 */
export function makeInitials(name?: string, fallback = 'U'): string {
  const s = (name ?? '').trim()
  if (!s) return fallback

  return s
    .split(' ')
    .filter(Boolean)
    .slice(0, 2)
    .map(part => part[0]?.toUpperCase())
    .join('')
}
