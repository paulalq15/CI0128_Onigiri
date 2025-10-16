<template>
  <div class="d-flex flex-column">
    <div class="container mt-5">
      <h1 class="display-4 text-center">Creación de planilla</h1>

      <div class="row justify-content-end">
        <div class="col-2"></div>
      </div>

      <table class="table is-bordered is-striped is-narrow is-hoverable is-fullwidth">
        <thead>
          <tr>
            <th>Fecha de pago</th>
            <th>Total salarios brutos</th>
            <th>Total deducciones empleador</th>
            <th>Total deducciones empleado</th>
            <th>Total beneficios</th>
            <th>Total salarios netos</th>
            <th>Total costo planilla</th>
          </tr>
        </thead>

        <tbody>
          <tr v-for="(employee, index) of employees" :key="index">
            <td>{{ employee.idCard }}</td>
            <td>{{ employee.name1 }}</td>
            <td>{{ employee.name2 }}</td>
            <td>{{ employee.surname1 }}</td>
            <td>{{ employee.surname2 }}</td>
            <td>{{ this.formatDate(employee.birthdayDate) }}</td>
            <td>{{ employee.surname2 }}</td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script>
import axios from "axios";
import { getUser } from '../session.js';

export default {
  name: "EmployeesList",

  data() {
    return {
      employees: [],
      user: null,
    };
  },

  methods: {
    getEmployees() {
      const user = getUser();

      if (!user) {
        return;
      }

      this.user = user;
      const companyId = user.companyUniqueId;
      const url = `https://localhost:7071/api/PersonUser/getEmployeesByCompanyId?companyId=${companyId}`;

      axios.get(url).then((response) => {
        this.employees = response.data;
      });
    },

    formatDate(dateString) {
      const date = new Date(dateString);
      return date.toLocaleDateString();
    }
  },

  created: function () {
    this.getEmployees();
  },
};

</script>

<style lang="scss" scoped></style>