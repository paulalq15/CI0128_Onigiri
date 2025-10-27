<template>
<!-- Restrict page if user is Employee or Admin -->
<div
  v-if="$session.user?.typeUser === 'Empleado' || $session.user?.typeUser === 'Administrador'"
  class="d-flex flex-column text-center"
>
  <h3>Acceso restringido</h3>
  <p>Esta página no está disponible</p>
</div>

<!-- Everything else must be inside v-else to be the sibling condition -->
<div v-else class="d-flex flex-column">
  <div class="container mt-5">
    <h1 class="display-4 text-center">Deducciones Disponibles</h1>
    <div class="row justify-content-end">
      <div class="col-2"></div>
    </div>

    <table class="table is-bordered is-striped is-narrow is-hoverable is-fullwidth">
      <thead>
        <tr>
          <th>Nombre</th>
          <th>Tipo de Cálculo</th>
          <th>Valor</th>
          <th>Acción</th>
        </tr>
      </thead>

      <tbody>
        <tr v-for="(deduction, index) in filteredDeductions" :key="index">
          <td>{{ deduction.elementName }}</td>
          <td>{{ deduction.calculationType }}</td>
          <td>{{ deduction.calculationValue }}</td>
          <td>
            <button
              class="btn btn-secondary btn-sm"
              @click="addAppliedElement(index, deduction.elementId)"
            >
              Seleccionar
            </button>
          </td>
        </tr>

        <tr>
          <td
            style="text-align: center; width: 50px; height: 50px; border: 1px solid #000; font-weight: bold; vertical-align: middle;"
            colspan="4">
            Total de Deducciones: {{ filteredDeductions.length }}
          </td>
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
    name: "DeductionsList",

    data() {
      return {
        deductions: [],
        appliedElements: [],
        companyId: null,
        user: null,
      };
    },

    computed: {
      filteredDeductions() {
        return this.deductions.filter(deduction => deduction.paidBy === "Empleado");
      }
    },

    methods: {
      async getCompanyIdByUserId() {
        try {
          const response = await axios.get(`https://localhost:7071/api/Company/getCompanyIdByUserId?userId=${this.user.userId}`);
          this.companyId = response.data;
          this.getDeductions();

        } catch (error) {
          console.error("Error getting companyId:", error);
        }
      },

      getDeductions() {
        if (!this.companyId) {
          console.warn("CompanyId is not set yet");
          return;
        }

        axios.get(`https://localhost:7071/api/PayrollElement/GetPayRollElements`, {
          params: {
            idCompany: this.companyId
          }})

          .then((response) => {
            this.deductions = response.data;
          })

          .catch(error => {
            console.error("Error fetching deductions:", error);
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

      getTotalActiveAppliedElements() {
        return this.appliedElements.filter(element => element.status === "Activo").length;
      },

      addAppliedElement(index, elementId) {
        // Verify that the benefit hasn't been selected yet:
        const selectedDeductionName = this.filteredDeductions[index].elementName;

        const alreadySelected = this.appliedElements.some(
        (applied) => applied.elementName == selectedDeductionName);

        if (alreadySelected) {
          alert("Esta deducción ya está seleccionada.");
          return;
        }

        if (!this.user || !this.user.userId) {
          alert("User ID no está definido");
          return;
        }

        if (!elementId) {
          alert("Element ID no está definido");
          return;
        }

        // Make a POST request to add the new applied element:
        axios.post(`https://localhost:7071/api/AppliedElement/addAppliedElement`,
        {
          UserId: this.user.userId,
          ElementId: elementId
        })

        .then(response => {
        alert("Deducción agregada exitosamente.");

        // Update the appliedElements list and totals after the successful addition:
        this.appliedElements.push(response.data);
        window.location.reload();
        })

        .catch(error => {
          alert("Error al agregar la deducción.", error);
        });
      },

      async deactivateAppliedElement(appliedElementId, status) {
        if (status == "Inactivo") {
          alert("El elemento seleccionado ya está inactivo.");
          return;
        }

        try {
          await axios.post('https://localhost:7071/api/AppliedElement/deactivateAppliedElement', {
            ElementId: appliedElementId
          });

          window.location.reload();
        }

        catch (error) {
          if (error.code === 'ECONNABORTED') {
            console.error('Request aborted or timed out');
          } 

          else {
            console.error('Error in request:', error.message);
          }
        }
      }
    },

    created() {
      this.user = getUser();
      this.getCompanyIdByUserId();
      this.getAppliedElements();
    },
  };

</script>

<style lang="scss" scoped></style>
