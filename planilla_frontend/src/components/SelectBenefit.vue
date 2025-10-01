<template>
  <!-- Mostrar mensaje de acceso restringido si es Administrador -->
  <div v-if="$session.user?.typeUser === 'Administrador'" class="d-flex flex-column text-center">
    <h3>Acceso restringido</h3>
    <p>Esta página no está disponible</p>
  </div>

  <!-- Mostrar tabla solo si es Empleado -->
  <div v-else-if="$session.user?.typeUser === 'Empleado'" class="d-flex flex-column">
    <div class="container mt-5">
      <h1 class="display-4 text-center">Beneficios Disponibles</h1>

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
          <tr v-for="(benefit, index) of filteredBenefits" :key="index">
            <td>{{ benefit.elementName }}</td>
            <td><button class="btn btn-secondary btn-sm" @click="addAppliedElement(index)">Seleccionar</button></td>
          </tr>

          <!-- Fila con contador total -->
          <tr>
            <td style="text-align: center; width: 50px; height: 50px; border: 1px solid #000; font-weight: bold; vertical-align: middle;">
              Total Benefits: {{ filteredBenefits.length }}
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div class="container mt-5">
      <h1 class="display-4 text-center">Beneficios Aplicados</h1>

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
            <td>{{ this.formatDate(appliedElement.startDate) }}</td>
            <td>{{ this.formatDate(appliedElement.endDate) }}</td>
            <td>{{ appliedElement.status }}</td>
            <td><button class="btn btn-danger btn-sm">Desactivar</button></td>
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
        totalCompanyBenefits: 0,
        totalSelectedEmployeeBenefits: 0,
        companyId: null,
        user: null,
      };
    },

    computed: {
      filteredBenefits() {
        return this.benefits.filter(benefit => benefit.paidBy === "Empleador");
      }
    },

    methods: {
      async getCompanyIdByUserId() {
        try {
          const response = await axios.get(`https://localhost:7071/api/Company/getCompanyIdByUserId?userId=${this.user.userId}`);
          this.companyId = response.data;
          this.getBenefits();
        } catch (error) {
          console.error("Error getting companyId:", error);
        }
      },

      getBenefits() {
        if (!this.companyId) {
          console.warn("CompanyId is not set yet");
          return;
        }

        axios.get(`https://localhost:7071/api/PayrollElement/GetPayRollElements`, {
          params: {
            idCompany: this.companyId
          }})

          .then((response) => {
            this.benefits = response.data;
          })
          .catch(error => {
            console.error("Error fetching benefits:", error);
          });
      },

      getCompanyTotalBenefitsElements() {
        axios.get(`https://localhost:7071/api/getCompanyTotalBenefitsByCompanyId?CompanyId=${this.companyId}`)
          .then((response) => {
            this.appliedElements = response.data;
          })
          .catch(error => {
            console.error("Error getting applied elements:", error);
          });
      },

      getAppliedElements() {
        axios.get(`https://localhost:7071/api/AppliedElement/getAppliedElements?employeeId=${this.user.userId}`)
          .then((response) => {
            this.appliedElements = response.data;
          })
          .catch(error => {
            console.error("Error getting applied elements:", error);
          });
      },

      formatDate(dateString) {
        if (dateString == null) {
          return "-";
        }
        const date = new Date(dateString);
        return date.toLocaleDateString();
      },

      addAppliedElement(index) {
        if (this.totalSelectedEmployeeBenefits == this.totalCompanyBenefits) {
          alert("ALERTA: Se llegó al máximo de beneficios disponibles");
          return;
        }

        // Verify that the benefit hasn't been selected yet:
        const selectedBenefitName = this.filteredBenefits[index].elementName;

        // Verificar si el beneficio ya está aplicado
        const alreadySelected = this.appliedElements.some(
        (applied) => applied.elementName === selectedBenefitName
        );

        if (alreadySelected) {
          alert("Este beneficio ya está seleccionado.");
          return;
        }
      }
    },

    created() {
      this.user = getUser();
      this.getCompanyIdByUserId(); // Fetch companyId, then getBenefits will be called internally
      this.getAppliedElements(); // Can be called immediately as it depends only on userId
    },
  };
</script>


<style lang="scss" scoped></style>
