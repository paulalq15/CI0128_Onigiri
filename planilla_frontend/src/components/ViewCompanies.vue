<template>
  <div>
    <!-- Restrict page if user is not 'Empleador' -->
    <div v-if="$session.user?.typeUser !== 'Empleador'" class="d-flex flex-column text-center">
      <h3>Acceso restringido</h3>
      <p>Esta página no está disponible</p>
    </div>

    <!-- Show page content only if user is 'Empleador' -->
    <div v-else class="d-flex flex-column">
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
              <th>Empleador</th>
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
              <td>{{ employeeCounts[String(company.companyId)] || 0 }}</td>
              <td>{{  }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script>
import axios from "axios";
import { getUser } from '../session.js';

export default {
  name: "CompaniesList",

  data() {
    return {
      companies: [],
      employeeCounts: {},
    };
  },

  computed: {
    user() {
      return getUser();
    },
  },

  methods: {
    getCompanies() {
      axios.get(`https://localhost:7071/api/Company/getCompanies/?employerId=${this.user.userId}`)
        .then((response) => {
        this.companies = response.data;
        this.companies.forEach(company => {
          this.getTotalEmployees(company.companyId);
        });
        })
        .catch(error => {
          console.error("Error al obtener compañías:", error);
        });
    },

    getTotalEmployees(companyId) {
      axios.get(`https://localhost:7071/api/Company/getTotalEmployees/?companyId=${companyId}`)
      .then((response) => {
      console.log("Employees API response:", response.data, typeof response.data);
      const total = parseInt(response.data, 10) || 0;
      this.$set(this.employeeCounts, String(companyId), total);
      })
      .catch((error) => {
        console.error("Error al obtener el total de empleados:", error);
      });
    }
  },

  created() {
    this.getCompanies();
  },
};
</script>

<style lang="scss" scoped></style>
