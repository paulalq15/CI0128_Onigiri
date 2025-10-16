<template>
  <div class="d-flex flex-column">
    <h1 class="text-center">Crear beneficios y deducciones</h1>

    <div class="container py-4 flex-fill d-flex justify-content-center">
      <form
        ref="elementForm"
        @submit.prevent="saveElement"
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
            v-model="name"
          />
          <div class="invalid-feedback">
            Ingrese el nombre, máximo {{ maxNameLength }} caracteres.
          </div>
        </div>

        <div class="mb-3 col-xl-6 col-sm-12">
          <label for="calculationType" class="form-label required">Tipo de cálculo</label>
          <select
            class="form-select"
            id="calculationType"
            required
            v-model="calculationType"
            @change="cleanValue"
          >
            <option value="Monto">Monto</option>
            <option value="Porcentaje">Porcentaje</option>
            <option value="API">API</option>
          </select>
          <div class="invalid-feedback">Seleccione el tipo de cálculo</div>
        </div>

        <!--Calculation values, change API IDs ***-->
        <div v-if="calculationType === 'API'" class="mb-3 col-xl-6 col-sm-12">
          <label class="form-label required">API</label>
          <select class="form-select" v-model="calculationValue" required>
            <option selected disabled value="">Seleccione un API</option>
            <option value="1">Asociación Solidarista</option>
            <option value="2">Seguro Privado</option>
            <option value="3">Pensión Voluntaria</option>
          </select>
          <div class="invalid-feedback">Seleccione un API</div>
        </div>
        <div v-else-if="calculationType === 'Porcentaje'" class="mb-3 col-xl-6 col-sm-12">
          <label class="form-label required">Valor</label>
          <div class="input-group">
            <input
              type="number"
              class="form-control"
              v-model="calculationValue"
              min="0"
              :max="maxPercentaje"
              step="0.01"
              required
              @input="clampPercent"
            />
            <span class="input-group-text">%</span>
            <div class="invalid-feedback">Ingrese el porcentaje entre 0 y {{ maxPercentaje }}</div>
          </div>
        </div>
        <div v-else class="mb-3 col-xl-6 col-sm-12">
          <label class="form-label required">Valor</label>
          <div class="input-group">
            <span class="input-group-text">₡</span>
            <input
              type="number"
              class="form-control"
              v-model="calculationValue"
              min="0"
              step="0.01"
              required
              :disabled="!calculationType"
            />
            <div class="invalid-feedback">Ingrese el monto con un máximo de 2 decimales</div>
          </div>
        </div>

        <!--Paid by and element type only if calculation Monto or Porcentaje-->
        <div
          v-if="calculationType === 'Monto' || calculationType === 'Porcentaje'"
          class="mb-3 col-xl-6 col-sm-12"
        >
          <label for="PaidBy" class="form-label required">Pagado por</label>
          <select
            class="form-select"
            id="PaidBy"
            required
            v-model="paidBy"
            @change="addElementType"
          >
            <option value="Empleado">Empleado</option>
            <option value="Empleador">Empleador</option>
          </select>
          <div class="invalid-feedback">Seleccione el responsable del pago.</div>
        </div>
        <div
          v-if="calculationType === 'Monto' || calculationType === 'Porcentaje'"
          class="mb-3 col-xl-6 col-sm-12"
        >
          <label for="elementType" class="form-label">Tipo de elemento</label>
          <input
            type="text"
            class="form-control"
            id="elementType"
            disabled
            readonly
            v-model="elementType"
          />
        </div>

        <div class="mb-3">
          <button class="btn btn-custom w-100 mt-2" type="submit">Crear elemento</button>
        </div>
      </form>
    </div>

    <!--Toast alerts-->
    <div class="toast-container position-fixed top-0 end-0 p-3">
      <div
        v-if="showToast"
        class="toast show align-items-center text-white border-0"
        :class="toastType"
        role="alert"
        aria-live="assertive"
        aria-atomic="true"
      >
        <div class="d-flex">
          <div class="toast-body">
            {{ toastMessage }}
          </div>
          <button
            type="button"
            class="btn-close btn-close-white me-2 m-auto"
            @click="showToast = false"
          ></button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import URLBaseAPI from '../axiosAPIInstances.js';
export default {
  data() {
    return {
      name: '',
      paidBy: '',
      elementType: '',
      calculationType: '',
      calculationValue: '',
      companyId: '',
      userId: '',
      maxNameLength: 40,
      maxPercentaje: 100,
      showToast: false,
      toastMessage: '',
      toastType: 'bg-success',
      toastTimeout: 3000,
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

    addElementType() {
      this.elementType = this.paidBy === 'Empleado' ? 'Deducción' : 'Beneficio';
    },

    cleanValue() {
      this.calculationValue = '';
    },

    saveElement() {
      var self = this;
      var form = this.$refs.elementForm;

      if (!form.checkValidity()) {
        return;
      }

      URLBaseAPI.post('/api/PayrollElement', {
        elementName: this.name,
        paidBy: this.calculationType === 'API' ? 'Empleador' : this.paidBy,
        calculationType: this.calculationType,
        // calculationValue: Number(this.calculationValue || 0),
        calculationValue: this.calculationType === 'API' ? Number(this.calculationValue) : Number(this.calculationValue || 0),
        companyId: Number(this.$session.user?.companyUniqueId),
        userId: Number(this.$session.user?.userId),
        status: 'Active',
      })
        .then(function () {
          self.toastMessage = 'Elemento creado correctamente';
          self.toastType = 'bg-success';
          self.showToast = true;
          setTimeout(function () {
            self.showToast = false;
            window.location.href = '/app/Home';
          }, self.toastTimeout);
        })
        .catch(function (error) {
          var msg =
            error && error.response && error.response.data
              ? error.response.data
              : 'Error inesperado al crear el elemento de planilla';
          self.toastMessage = msg;
          self.toastType = 'bg-danger';
          self.showToast = true;

          setTimeout(function () {
            self.showToast = false;
          }, self.toastTimeout);
        });
    },
  },

  mounted() {
    if (this.$session.user?.typeUser !== 'Empleador') return;
    this.initBootstrapValidation();
  },

  watch: {
    calculationValue(newVal) {
      if (this.calculationType === 'API') {
        switch(newVal) {
          case '1':
            this.name = 'Asociación Solidarista';
            break;

          case '2':
            this.name = 'Seguro Privado';
            break;

          case '3':
            this.name = 'Pensión Voluntaria';
            break;

          default:
            this.name = '';
        }
      }
    }
  }
};

</script>

<style lang="scss" scoped>
.form-label.required::after {
  content: ' *';
  color: red;
}

.btn-custom {
  background-color: #234d34;
  color: #fff;
  border: none;
  border-radius: 0.375rem;
  padding: 0.5rem 1rem;
  font-weight: 600;
  text-align: center;
}

.btn-custom:hover {
  background-color: #1b3d2a;
  color: #fff;
}

</style>
