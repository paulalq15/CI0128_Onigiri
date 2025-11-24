<template>
  <div class="d-flex flex-column">
    <div class="container mt-5">
      <h1 class="display-4 text-center">Empresas activas</h1>
      <transition name="fade">
        <PopUp
          v-if="popup"
          :mainText="popUpData.mainText"
          :bodyText="popUpData.bodyText"
          :fistButtontext="popUpData.fistButtontext"
          :secondButtontext="popUpData.secondButtontext"
          @closePopUp="togglePopUp"
          @resolved="popUpData.resolve"
        />
      </transition>
      <table class="table is-bordered is-striped is-narrow is-hoverable is-fullwidth">
        <thead>
          <tr>
            <th>Cédula Jurídica</th>
            <th>Nombre</th>
            <th>Teléfono</th>
            <th>Cantidad de Beneficios</th>
            <th>Frecuencia de Pago</th>
            <th>Día de Pago #1</th>
            <th>Día de Pago #2</th>
            <th>Total Empleados</th>
            <th v-if="$session.user?.typeUser === 'Administrador'">Employer</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(company, index) in activeCompanies" :key="index">
            <td>{{ company.companyId }}</td>
            <td>{{ company.companyName }}</td>
            <td>{{ company.telephone }}</td>
            <td>{{ company.maxBenefits }}</td>
            <td>{{ company.paymentFrequency }}</td>
            <td>{{ company.payDay1 }}</td>
            <td>{{ company.payDay2 }}</td>
            <td>{{ company.employeeCount }}</td>
            <td v-if="$session.user?.typeUser === 'Administrador'">{{ company.employerName }}</td>
            <td><button class="btn btn-sm btn-danger" @click="deleteCompany(company)">Eliminar</button></td>
          </tr>
        </tbody>
      </table>
    </div>

    <div class="container mt-5" v-if="inactiveCompanies.length > 0">
      <h1 class="display-4 text-center">Empresas inactivas</h1>
      <table class="table is-bordered is-striped is-narrow is-hoverable is-fullwidth">
        <thead>
          <tr>
            <th>Cédula Jurídica</th>
            <th>Nombre</th>
            <th>Teléfono</th>
            <th>Cantidad de Beneficios</th>
            <th>Frecuencia de Pago</th>
            <th>Día de Pago #1</th>
            <th>Día de Pago #2</th>
            <th>Total Empleados</th>
            <th v-if="$session.user?.typeUser === 'Administrador'">Employer</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(company, index) in inactiveCompanies" :key="index">
            <td>{{ company.companyId }}</td>
            <td>{{ company.companyName }}</td>
            <td>{{ company.telephone }}</td>
            <td>{{ company.maxBenefits }}</td>
            <td>{{ company.paymentFrequency }}</td>
            <td>{{ company.payDay1 }}</td>
            <td>{{ company.payDay2 }}</td>
            <td>{{ company.employeeCount }}</td>
            <td v-if="$session.user?.typeUser === 'Administrador'">{{ company.employerName }}</td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script>
import { useGlobalAlert } from '../utils/alerts.js';

import PopUp from './alerts/PopUp.vue';

import URLBaseAPI from '../axiosAPIInstances.js';

export default {
  name: "CompaniesList",
  components: {
    PopUp,
  },

  data() {
    return {
      activeCompanies: [],
      inactiveCompanies: [],
      popup: false,
      popUpData: {
        mainText: '',
        bodyText: '',
        fistButtontext: '',
        secondButtontext: '',
      },
    };
  },

  methods: {
    fetchCompanies() {
      const userId = this.$session.user?.userId;
      if (!userId) return;

      URLBaseAPI.get('/api/Company/getCompaniesWithStats', {
        params: {
          employerId: userId,
          viewerUserId: userId,
        },
      })
      .then((response) => {
        this.updateCompaniesLists(response.data);
      })
      .catch((error) => {
        console.error('Error fetching companies:', error);
      });
    },

    async deleteCompany(company) {
      const ok = await this.showPopUp(company);

      if (!ok) return;

      try {
        await URLBaseAPI.delete('/api/Company/company', {
          params: { companyId: company.companyUniqueId , employeerPersonId: this.$session.user?.personId },
        });

        const alert = useGlobalAlert();
        alert.show('Empresa eliminada exitosamente', 'success');

        this.fetchCompanies();
      } catch (error) {
          const alert = useGlobalAlert();
          const message = error.response?.data?.message|| 'Error desconocido';
          alert.show('Error al eliminar la empresa: ' + message, 'warning');
      }
    },

    updateCompaniesLists(data) {
      this.activeCompanies = data.filter(company => company.state === 'Activo');
      this.inactiveCompanies = data.filter(company => company.state === 'Inactivo');
    },

    showPopUp(company) {
      return new Promise((resolve) => {

        this.popUpData = {
          mainText: 'Eliminar Empresa',
          bodyText: `¿Está seguro de que desea eliminar esta empresa?
            Empresa: ${company.companyName} (Cédula Jurídica: ${company.companyId})`,
          fistButtontext: 'Sí, eliminar',
          secondButtontext: 'No, cancelar',
          resolve,
        };

        this.togglePopUp();
      });
    },

    togglePopUp() {
      this.popup = !this.popup;
    }
  },

  created() {
    this.fetchCompanies();
  },
};
</script>

<style lang="scss" scoped>
  table {
    border-radius: 20px;
  }
</style>
