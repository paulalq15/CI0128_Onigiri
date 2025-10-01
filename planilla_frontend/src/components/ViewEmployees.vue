<template>
  <!-- Restrict page if user is Admin or Employee -->
  <div v-if="$session.user?.typeUser === 'Administrador' || $session.user?.typeUser === 'Empleado'" 
       class="d-flex flex-column text-center">
    <h3>Acceso restringido</h3>
    <p>Esta página no está disponible</p>
  </div>

  <!-- Show the employees table only if user is NOT Admin or Employee -->
  <div v-else class="d-flex flex-column">
    <div class="container mt-5">
      <h1 class="display-4 text-center">Lista de Empleados</h1>

      <div class="row justify-content-end">
        <div class="col-2"></div>
      </div>

      <table class="table is-bordered is-striped is-narrow is-hoverable is-fullwidth">
        <thead>
          <tr>
            <th>Cédula</th>
            <th>Primer Nombre</th>
            <th>Segundo Nombre</th>
            <th>Primer Apellido</th>
            <th>Segundo Apellido</th>
            <th>Fecha Nacimiento</th>
            <th>Email</th>
            <th>Tipo Contrato</th>
            <th>Puesto</th>
            <th>Departamento</th>
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
            <td>{{ employee.email }}</td>
            <td>{{ employee.contractType }}</td>
            <td>{{ employee.jobPosition }}</td>
            <td>{{ employee.department }}</td>
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
