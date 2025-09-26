export const STORAGE_KEY = "onigiri_user";

export function setUser(user) {
  localStorage.setItem(STORAGE_KEY, JSON.stringify(user));
}

export function getUser() {
  try {
    const raw = localStorage.getItem(STORAGE_KEY);
    if (!raw) return null;
    const u = JSON.parse(raw);
    if (u && typeof u === 'object' && (u.userId || u.email || u.fullName)) {
      return u;
    }
    return null;
  } catch {
    return null;
  }
}

export function clearUser() {
  localStorage.removeItem(STORAGE_KEY);
}

export function isAuthed() {
  const u = getUser();
  return !!(u && u.userId);
}