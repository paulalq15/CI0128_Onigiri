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
      <!-- Employee dropdown: -->
      <div class="mb-4">
        <label for="employeeSelect">Seleccione un empleado:</label>
        <select id="employeeSelect" v-model="selectedEmployee.id" @change="updateSelectedEmployeeInformation" class="form-control">
          <option disabled value="">Seleccione un empleado</option>
          <option v-for="(employee, index) in employees" :key="index" :value="employee.idUser">
            {{ "Id Usuario: " + employee.idUser + " " }}
            {{ employee.name1 }}
            {{ employee.name2 }}
            {{ employee.surname1 }}
            {{ employee.surname2 }}
          </option>
        </select>

      </div>
    </div>

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
              @click="addAppliedElement(index, deduction.idElement)">
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

  <div v-if="selectedEmployee.id" class="container mt-5">
      <h1 class="display-4 text-center">Deducciones Aplicadas</h1>

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
          <tr v-for="(appliedElement, index) of filteredAppliedElements" :key="index">
            <td>{{ appliedElement.elementName }}</td>
            <td>{{ formatDate(appliedElement.startDate) }}</td>
            <td>{{ formatDate(appliedElement.endDate) }}</td>
            <td>{{ appliedElement.status }}</td>
            <td><button class="btn btn-danger btn-sm" @click="deactivateAppliedElement(appliedElement.elementId, appliedElement.status)">
              Desactivar
            </button>
          </td>
        </tr>

          <tr>
            <td style="text-align: center; width: 50px; height: 50px; border: 1px solid #000; font-weight: bold; vertical-align: middle;" colspan="5">
              Total Deducciones Activas: {{ this.getTotalActiveAppliedElements() }}
            </td>
          </tr>
        </tbody>
      </table>
      
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
        employees: [],
        user: null,

        selectedEmployee: {
          id: null,
          name: ''
        },
      };
    },

    computed: {
      filteredDeductions() {
        return this.deductions.filter(deduction => deduction.paidBy === "Empleado");
      },

      deductionElementIds() {
        return new Set(this.filteredDeductions.map(d => d.idElement));
      },

      filteredAppliedElements() {
        return this.appliedElements.filter(applied =>
        this.deductionElementIds.has(applied.elementId)
      );
    },
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
        axios.get(`https://localhost:7071/api/AppliedElement/getAppliedElements?employeeId=${this.selectedEmployee.id}`)
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
        // Verify that an employee was selected:
        if (!this.selectedEmployee || !this.selectedEmployee.id) {
          alert("Por favor, seleccione un empleado antes de seleccionar una deducción.");
          return;
        }

        // Verify that the benefit hasn't been selected yet:
        const selectedDeductionName = this.filteredDeductions[index].elementName;

        const alreadySelected = this.appliedElements.some(
        (applied) => applied.elementName == selectedDeductionName && applied.elementStatus == "Activo");

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
          UserId: this.selectedEmployee.id,
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
      },

      updateSelectedEmployeeInformation() {
        const employee = this.employees.find(e => e.idUser === this.selectedEmployee.id);

        if (employee) {
          // Update selectedEmployee name
          this.selectedEmployee.name = employee.name1 + ' ' + (employee.name2 || '') + ' ' + employee.surname1 + ' ' + (employee.surname2 || '');

          alert(`Empleado seleccionado: ${this.selectedEmployee.name}`);

          // Fetch applied elements for selected employee:
          this.getAppliedElements();
        }
      }
    },

    created() {
      this.user = getUser();
      this.getCompanyIdByUserId();
      this.getEmployees();
    },
  };

</script>

<style lang="scss" scoped></style>
