export const STORAGE_KEY = "onigiri_user";

export function setUser(user) {
  localStorage.setItem(STORAGE_KEY, JSON.stringify(user));
}

export function getUser() {
  try {
    return JSON.parse(localStorage.getItem(STORAGE_KEY) || "null");
  } catch {
    return null;
  }
}

export function clearUser() {
  localStorage.removeItem(STORAGE_KEY);
}

export function isAuthed() {
  return !!getUser();
}
