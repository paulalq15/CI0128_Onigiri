import { reactive } from 'vue';
import { STORAGE_KEY, getUser, setUser, clearUser } from '../session';

const session = reactive({
  user: getUser(),
  set(u) {
    this.user = u;
    setUser(u);
  },
  clear() {
    this.user = null;
    clearUser();
  },
});

window.addEventListener('storage', (e) => {
  if (e.key === STORAGE_KEY) session.user = getUser();
});

export function useSession() {
  return session;
}