<template>
  <div class="d-flex flex-column">
    <h1 class="text-center">Editar - {{ payrollElement.elementType }}</h1>

    <div class="container py-4 flex-fill d-flex justify-content-center">
      <form
        ref="elementForm"
        @submit.prevent="updatePayrrollElement"
        class="row g-3 needs-validation"
        novalidate
        style="width: 800px"
      >

        <div class="mb-3">
          <label for="elementName" class="form-label required">Nombre</label>
          <input
            type="text"
            class="form-control"
            id="elementName"
            required
            :maxlength="maxNameLength"
            v-model="payrollElement.elementName"
          />
          <div class="invalid-feedback">
            Ingrese el nombre, máximo {{ maxNameLength }} caracteres.
          </div>
        </div>

        <div class="mb-3 col-xl-6 col-sm-12">
          <label for="calculationType" class="form-label">Tipo de cálculo</label>
          <input
            type="text"
            class="form-control disabled"
            id="calculationType"
            v-model="payrollElement.calculationType"
            disabled/>
        </div>

        <!--Calculation values, change API IDs ***-->
        <div v-if="payrollElement.calculationType === 'API'" class="mb-3 col-xl-6 col-sm-12">
          <label class="form-label">API</label>
          <input
            type="text"
            class="form-control disabled"
            v-model="payrollElement.calculationValue"
            disabled
            />
        </div>

        <div v-else-if="payrollElement.calculationType === 'Porcentaje'" class="mb-3 col-xl-6 col-sm-12">
          <label class="form-label">Valor</label>
          <div class="input-group">
            <input
              type="number"
              class="form-control disabled"
              v-model="payrollElement.calculationValue"
              disabled
            />
            <span class="input-group-text">%</span>
          </div>
        </div>

        <div v-else class="mb-3 col-xl-6 col-sm-12">
          <label class="form-label">Valor</label>
          <div class="input-group">
            <span class="input-group-text">₡</span>
            <input
              type="number"
              class="form-control disabled"
              v-model="payrollElement.calculationValue"
              disabled
            />
          </div>
        </div>

        <!--Paid by and element type only if calculation Monto or Porcentaje-->
        <div
          v-if="payrollElement.calculationType === 'Monto' || payrollElement.calculationType === 'Porcentaje'"
          class="mb-3 col-xl-6 col-sm-12"
        >
          <label for="PaidBy" class="form-label">Pagado por</label>
          <input
            type="text"
            class="form-control disabled"
            id="PaidBy"
            v-model="payrollElement.paidBy"
            disabled
          />
        </div>

        <div
          v-if="payrollElement.calculationType === 'Monto' || payrollElement.calculationType === 'Porcentaje'"
          class="mb-3 col-xl-6 col-sm-12"
        >
          <label for="elementType" class="form-label">Tipo de elemento</label>
          <input
            type="text"
            class="form-control disabled"
            id="elementType"
            disabled
            v-model="payrollElement.elementType"
          />
        </div>

        <div class="mb-3 pair-btn">
          <LinkButton text="Cancelar" @click="goBack" />
          <LinkButton text="Guardar" @click="updatePayrrollElement" />
        </div>
      </form>
    </div>
  </div>
</template>

<script>
import { useRoute } from 'vue-router';
import { useGlobalAlert } from '@/utils/alerts.js';

import URLBaseAPI from '../axiosAPIInstances.js';

import LinkButton from './LinkButton.vue';


export default {
  components: {
    LinkButton
  },
  
  data() {
    return {
      payrollElement : {
        elementId: 0,
        elementName: '',
        calculationType: '',
        calculationValue: '',
        paidBy: '',
        elementType: '',
        companyId: '',
      },

      maxNameLength: 40,
    };
  },
  
  methods: {
    initBootstrapValidation() {
      const form = this.$refs.elementForm;
      if (!form) return;
      
      form.addEventListener('submit', (event) => {
        if (!form.checkValidity()) {
          event.preventDefault();
          event.stopPropagation();
        } else {
          event.preventDefault();
        }
        form.classList.add('was-validated');
      });
    },
    
    setPayrollElementData() {
      const route = useRoute();
      const payrollId = route.params.PEId;
      
      URLBaseAPI.get(`/api/PayrollElement/${payrollId}`)
          .then((response) => {
            this.payrollElement = response.data;
            this.payrollElement.elementType = this.payrollElement.paidBy === 'Empleador' ? 'Beneficio' : 'Deducción';
            console.log(this.payrollElement);
          })
          .catch((error) => {
            const alert = useGlobalAlert();

            alert.show("Error al mostrar la información. Detalle: " + error.response?.data?.message || error.message, 'warning');
            window.history.back();
          });
    },
    
    goBack() {
      window.history.back();
    },

    updatePayrrollElement() {
      URLBaseAPI.post('/api/PayrollElement/updPayrollElement', this.payrollElement)
          .then((response) => {
            const alert = useGlobalAlert();

            alert.show(response.data.message, 'success');
          })
          .catch((error) => {
            const alert = useGlobalAlert();

            alert.show("Error al actualizar el elemento. Detalle: " + error.response?.data?.message || error.message, 'warning');
          });
    },
  },

  mounted() {
    if (this.$session.user?.typeUser !== 'Empleador') return;
    this.initBootstrapValidation();
    this.setPayrollElementData();
  },
};
</script>

<style lang="scss" scoped>
  .form-label.required::after {
    content: ' *';
    color: red;
  }

  .disabled {
    color: #718096;
  }

  .pair-btn {
    display: flex;
    gap: 20px;
  }
</style>
