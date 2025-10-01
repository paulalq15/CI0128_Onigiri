<template>
  <div>
    <div class="d-flex flex-column">
      <div class="container mt-5">
        <h1 class="display-4 text-center">Lista de Empresas</h1>
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
            <tr v-for="(company, index) in companies" :key="index">
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
  </div>
</template>

<script>
import URLBaseAPI from '../axiosAPIInstances.js';

export default {
  name: "CompaniesList",

  data() {
    return {
      companies: [],
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
        this.companies = Array.isArray(response.data) ? response.data : [];
      })
      .catch((error) => {
        console.error('Error fetching companies:', error);
      });
    },
  },

  created() {
    this.fetchCompanies();
  },
};
</script>

<style lang="scss" scoped></style>
