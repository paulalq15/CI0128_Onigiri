<template>
  <!-- Mostrar mensaje de acceso restringido si es Administrador -->
  <div v-if="$session.user?.typeUser === 'Administrador'" class="d-flex flex-column text-center">
    <h3>Acceso restringido</h3>
    <p>Esta página no está disponible</p>
  </div>

  <!-- Mostrar tabla solo si es Empleado -->
  <div v-else-if="$session.user?.typeUser === 'Empleado'" class="d-flex flex-column">
    <div class="container mt-5">
      <h1 class="display-4 text-center">Lista de Beneficios</h1>

      <div class="row justify-content-end">
        <div class="col-2"></div>
      </div>

      <table class="table is-bordered is-striped is-narrow is-hoverable is-fullwidth">
        <thead>
          <tr>
            <th>Nombre</th>
            <th>Acción</th>
          </tr>
        </thead>

        <tbody>
          <!-- Filas de beneficios -->
          <tr v-for="(benefit, index) of benefits" :key="index">
            <td>{{ benefit.elementName }}</td>
            <td><button class="btn btn-secondary btn-sm">Seleccionar</button></td>
          </tr>

          <!-- Fila con contador total -->
          <tr>
            <td style="text-align: center; width: 50px; height: 50px; border: 1px solid #000; font-weight: bold; vertical-align: middle;">
              Total Benefits: {{ benefits.length }}
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div class="container mt-5">
      <h1 class="display-4 text-center">Lista de Beneficios Aplicados</h1>

      <div class="row justify-content-end">
        <div class="col-2"></div>
      </div>

      <table class="table is-bordered is-striped is-narrow is-hoverable is-fullwidth">
        <thead>
          <tr>
            <th>Nombre</th>
            <th>Fecha Inicio</th>
            <th>Fecha Fin</th>
            <th>Estado</th>
            <th>Acción</th>
            
          </tr>
        </thead>

        <tbody>
          <!-- Filas de elementos aplicados -->
          <tr v-for="(appliedElement, index) of appliedElements" :key="index">
            <td>{{ appliedElement.elementName }}</td>
            <td>{{ appliedElement.startDate }}</td>
            <td>{{ appliedElement.endDate }}</td>
            <td>{{ appliedElement.status }}</td>
            <td><button class="btn btn-secondary btn-sm">Seleccionar</button></td>
          </tr>

          <!-- Fila con contador total -->
          <tr>
            <td style="text-align: center; width: 50px; height: 50px; border: 1px solid #000; font-weight: bold; vertical-align: middle;">
              Total Applied Elements: {{ appliedElements.length }}
            </td>
          </tr>
        </tbody>
      </table>
      
    </div>

  </div>

  <!-- Mensaje para otros tipos de usuario -->
  <div v-else class="d-flex flex-column text-center">
    <p>Acceso no autorizado</p>
  </div>
</template>


<script>
  import axios from "axios";
  import { getUser } from '../session.js';

  export default {
    name: "BenefitsList",

    data() {
      return {
        benefits: [],
        appliedElements: [],
        totalBenefits: 0,
        selectedBenefits: 0,
        user: getUser(),
      };
    },

    methods: {
      getBenefits() {
        axios.get(`https://localhost:7071/api/PayrollElement/getPayrollElements?paidBy=Empleador`)
        .then((response) => {
        this.benefits = response.data;

        }).catch(error => {
          console.error("Error al obtener los beneficios:", error);
        });
      },

      getAppliedElements() {
        var user = getUser();
        axios.get(`https://localhost:7071/api/AppliedElement/getAppliedElements?employeeId=${user.userId}`)
        .then((response) => {
        this.appliedElements = response.data;

        }).catch(error => {
          console.error("Error al obtener los elementos aplicados:", error);
        });
      },
    },

    created: function () {
      this.getBenefits();
      this.getAppliedElements();
    },
  };

</script>

<style lang="scss" scoped></style>
