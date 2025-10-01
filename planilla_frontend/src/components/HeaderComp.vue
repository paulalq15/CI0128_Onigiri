<template>
  <header class="d-flex justify-content-between align-items-center py-3 mb-4 px-4" id="headerComp">
    <a class="d-flex align-items-center mb-3 mb-md-0 me-md-auto link-body-emphasis text-decoration-none">
      <img src="@/assets/logo.png" alt="Logo" width="60" height="60" class="me-2" />
      <span class="fs-5 fw-medium">Onigiri Intelligent Solutions</span>
    </a>

    <ul class="nav nav-underline d-flex align-items-center gap-3">
      <!-- Dropdown de empresas -->
      <li class="nav-item" v-if="isLoggedIn && session.user?.typeUser === 'Empleador'">
        <div class="dropdown text-end">
          <button
            class="btn btn-link dropdown-toggle text-dark text-decoration-none shadow-none"
            type="button"
            data-bs-toggle="dropdown"
            aria-expanded="false"
          >
            <span class="fs-5 fw-medium">{{ headerCompanyTitle }}</span>
          </button>

          <ul class="dropdown-menu dropdown-menu-end" style="min-width: 260px;">
            <li v-if="loadingCompanies">
              <span class="dropdown-item disabled">Cargando…</span>
            </li>
            <li v-else-if="companies.length === 0">
              <span class="dropdown-item disabled">No tienes empresas</span>
            </li>
            <li v-else v-for="c in companies" :key="c.companyUniqueId">
              <a class="dropdown-item d-flex justify-content-between align-items-center"
                 href="#"
                 @click.prevent="selectCompany(c)">
                <span>{{ c.companyName }}</span>
                <span v-if="c.companyUniqueId === session.user?.companyUniqueId" class="badge bg-secondary">Activa</span>
              </a>
            </li>
            <li v-if="errorMsg">
              <hr class="dropdown-divider" />
              <span class="dropdown-item text-danger">{{ errorMsg }}</span>
            </li>
          </ul>
        </div>
      </li>

      <!-- Menú hamburguesa (sin cambios) -->
      <li class="nav-item">
        <button
          class="btn p-0 border-0 bg-transparent d-flex align-items-center shadow-none"
          type="button"
          data-bs-toggle="offcanvas"
          data-bs-target="#appSidebar"
          aria-controls="appSidebar"
        >
          <i class="bi bi-list fs-2"></i>
        </button>
      </li>
    </ul>
  </header>
</template>

<script>
import URLBaseAPI from '../axiosAPIInstances.js';
import { useSession } from '../utils/useSession.js';

export default {
  name: 'HeaderComp',
  setup() {
    const session = useSession(); // { user, set(), clear() }
    return { session };
  },
  data() {
    return {
      companies: [],
      loadingCompanies: false,
      errorMsg: '',
    };
  },
  computed: {
    isLoggedIn() {
      return !!this.session.user?.userId;
    },
    sessionUserId() {
      return this.session.user?.userId || null;
    },
    headerCompanyTitle() {
      const saved = this.session.user?.companyName;
      if (saved && saved.trim()) return saved;

      const id = this.session.user?.companyUniqueId;
      if (!id) return 'Seleccionar empresa';
      const match = this.companies?.find?.(c => c.companyUniqueId === id);
      return match?.companyName || 'Seleccionar empresa';
    },
  },

  mounted() {
    if (this.isLoggedIn && this.session.user?.typeUser === 'Empleador') {
      this.fetchCompanies();
    }
  },
  methods: {
    async fetchCompanies() {
      this.errorMsg = '';
      if (!this.sessionUserId) return;

      this.loadingCompanies = true;
      try {
        const { data } = await URLBaseAPI.get('/api/Company/by-user/' + this.sessionUserId + '?onlyActive=true');

        const rows = Array.isArray(data) ? data.slice() : [];
        this.companies = rows;

       // Si no hay empresa activa, preselecciona la primera y guarda también el NOMBRE
        if (!this.session.user?.companyUniqueId && this.companies.length > 0) {
          const first = this.companies[0];
          const curr = this.session.user || {};
          this.session.set({ ...curr, companyUniqueId: first.companyUniqueId, companyName: first.companyName });
        }
        // Si ya hay id activo pero no hay nombre en sesión, complétalo
        else if (this.session.user?.companyUniqueId && !this.session.user?.companyName) {
          const match = this.companies.find(c => c.companyUniqueId === this.session.user.companyUniqueId);
          if (match) {
            const curr = this.session.user || {};
            this.session.set({ ...curr, companyName: match.companyName });
          }
        }
      } catch (err) {
        this.errorMsg =
          err?.response?.status === 404
            ? 'Endpoint no encontrado (404).'
            : (err?.code === 'ERR_NETWORK' || err?.message?.includes('Network'))
            ? 'No hay conexión con el API.'
            : err?.response?.data?.message || 'Error al cargar empresas.';
      } finally {
        this.loadingCompanies = false;
      }
    },

    selectCompany(selectedCompany) {
      if (!selectedCompany) return;
      const curr = this.session.user || {};
      this.session.set({ ...curr, companyUniqueId: selectedCompany.companyUniqueId, companyName: selectedCompany.companyName });
    },
  },
};
</script>

<style lang="scss" scoped>
#headerComp {
  background: rgba(89, 109, 83, 0.2);
}
</style>
