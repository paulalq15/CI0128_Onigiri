<template>
  <div class="d-flex flex-column">
    <h1 class="text-center">Crear nueva empresa</h1>

    <div class="container py-4 flex-fill d-flex justify-content-center">
      <form
        @submit.prevent="saveCompany"
        class="row g-3 needs-validation"
        novalidate
        style="width: 800px"
      >
        <div class="mb-3 col-xl-3 col-sm-12">
          <label for="companyId" class="form-label required">Cédula Jurídica</label>
          <input
            type="text"
            class="form-control"
            id="companyId"
            placeholder="3-000-000000"
            required
            pattern="[0-9]{1}-[0-9]{3}-[0-9]{6}"
            v-model="companyId"
          />
          <div class="invalid-feedback">
            Ingrese la cédula jurídica con el formato requerido #-###-######
          </div>
        </div>
        <div class="mb-3 col-xl-9 col-sm-12">
          <label for="companyName" class="form-label required">Nombre Empresa</label>
          <input
            type="text"
            class="form-control"
            id="companyName"
            required
            maxlength="150"
            v-model="companyName"
          />
          <div class="invalid-feedback">Ingrese el nombre de la empresa</div>
        </div>

        <!--Address-->
        <div class="mb-3 col-xl-6 col-sm-12">
          <label for="province" class="form-label required">Provincia</label>
          <select
            class="form-select"
            id="province"
            required
            v-model="selectedProvince"
            @change="getCounties"
          >
            <option selected disabled value="">Seleccione una Provincia</option>
            <option v-for="p in provinces" :key="p.value" :value="p.value">{{ p.value }}</option>
          </select>
          <div class="invalid-feedback">Seleccione una provincia</div>
        </div>
        <div class="mb-3 col-xl-6 col-sm-12">
          <label for="county" class="form-label required">Cantón</label>
          <select
            class="form-select"
            id="county"
            required
            v-model="selectedCounty"
            @change="getDistricts"
            :disabled="!selectedProvince || counties.length === 0"
          >
            <option selected disabled value="">Seleccione un cantón</option>
            <option v-for="c in counties" :key="c.value" :value="c.value">{{ c.value }}</option>
          </select>
          <div class="invalid-feedback">Seleccione un cantón</div>
        </div>
        <div class="mb-3 col-xl-6 col-sm-12">
          <label for="district" class="form-label required">Distrito</label>
          <select
            class="form-select"
            id="district"
            required
            v-model="selectedDistrict"
            @change="getZipCode"
            :disabled="!selectedCounty || districts.length === 0"
          >
            <option selected disabled value="">Seleccione un distrito</option>
            <option v-for="d in districts" :key="d.value" :value="d.value">{{ d.value }}</option>
          </select>
          <div class="invalid-feedback">Seleccione un distrito</div>
        </div>
        <div class="mb-3 col-xl-6 col-sm-12">
          <label for="zipCode" class="form-label">Código Postal</label>
          <input
            type="text"
            class="form-control"
            id="zipCode"
            disabled
            readonly
            v-model="zipCode"
          />
        </div>
        <div class="mb-3">
          <label for="addressDetails" class="form-label required">Otras señas</label>
          <textarea
            class="form-control"
            id="addressDetails"
            rows="3"
            placeholder="Otras señas"
            required
            maxlength="250"
            v-model="addressDetails"
          ></textarea>
          <div class="invalid-feedback">Ingrese las otras señas</div>
        </div>

        <!--Telephone and benefits-->
        <div class="mb-3">
          <label for="telephone" class="form-label">Teléfono</label>
          <input
            type="text"
            class="form-control"
            id="telephone"
            placeholder="2222-2222"
            pattern="[0-9]{4}-[0-9]{4}"
            v-model="telephone"
          />
          <div class="invalid-feedback">
            Ingrese el número de teléfono con el formato requerido ####-####
          </div>
        </div>
        <div class="mb-3">
          <label for="maxBenefits" class="form-label required">Cantidad máxima de beneficios</label>
          <input
            type="number"
            class="form-control"
            id="maxBenefits"
            placeholder="0"
            min="0"
            required
            v-model.number="maxBenefits"
          />
          <div class="invalid-feedback">
            Ingrese la cantidad máxima de beneficios que puede seleccionar un empleado
          </div>
        </div>

        <!--Payment frequency and days-->
        <div class="mb-3">
          <label for="paymentFrequency" class="form-label required">Frecuencia de pago</label>
          <select class="form-select" id="paymentFrequency" required v-model="paymentFrequency">
            <option selected disabled value="">
              Selecciona la frecuencia de pago de la planilla
            </option>
            <option>Mensual</option>
            <option>Quincenal</option>
          </select>
          <div class="invalid-feedback">Seleccione la frecuencia de pago de planilla</div>
        </div>
        <div class="mb-3">
          <label for="payDay1" class="form-label required">Día de pago</label>
          <select
            class="form-select"
            id="payDay1"
            required
            v-model="payDay1"
            :disabled="!paymentFrequency"
          >
            <option selected disabled value="">Día de pago</option>
            <option v-for="day in 31" :key="day" :value="day">{{ day }}</option>
          </select>
          <div class="invalid-feedback">Seleccione el día de pago de la planilla</div>
        </div>
        <div v-if="paymentFrequency === 'Quincenal'" class="mb-3">
          <label for="payDay2" class="form-label required">Segundo día de pago</label>
          <select class="form-select" id="payDay2" required v-model="payDay2" :disabled="!payDay1">
            <option selected disabled value="">Segundo día de pago</option>
            <option v-for="day in daysAfterFirst" :key="day" :value="day">{{ day }}</option>
          </select>
          <div class="invalid-feedback">Seleccione el segundo día de pago de la planilla</div>
        </div>
        <div class="mb-3">
          <button class="btn btn-custom w-100 mt-2" type="submit">Crear Empresa</button>
        </div>
      </form>
    </div>

    <!--Toast messages-->
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
      provinces: [],
      selectedProvince: '',
      counties: [],
      selectedCounty: '',
      districts: [],
      selectedDistrict: '',
      zipCode: '',
      paymentFrequency: '',
      payDay1: '',
      payDay2: '',
      companyId: '',
      companyName: '',
      addressDetails: '',
      telephone: '',
      maxBenefits: '',
      showToast: false,
      toastMessage: '',
      toastType: 'bg-success',
      toastTimeout: 3000,
      lastMonthDay: 31,
    };
  },
  computed: {
    daysAfterFirst() {
      if (!this.payDay1) return [];
      return Array.from({ length: this.lastMonthDay - this.payDay1 }, (_, i) => this.payDay1 + i + 1);
    },
  },
  watch: {
    paymentFrequency(val) {
      if (val === 'Mensual') this.payDay2 = '';
    },
    payDay1() {
      if (this.payDay2 && Number(this.payDay2) <= Number(this.payDay1)) {
        this.payDay2 = '';
      }
    },
  },
  methods: {
    initBootstrapValidation() {
      const forms = this.$el.querySelectorAll('.needs-validation');

      Array.from(forms).forEach((form) => {
        form.addEventListener(
          'submit',
          (event) => {
            if (!form.checkValidity()) {
              event.preventDefault();
              event.stopPropagation();
            } else {
              event.preventDefault();
            }
            form.classList.add('was-validated');
          },
          false
        );
      });
    },
    getProvince() {
      URLBaseAPI.get('/api/CountryDivision/Provinces').then((response) => {
        this.provinces = response.data;
      });
    },
    getCounties() {
      this.selectedCounty = '';
      this.counties = [];
      this.selectedDistrict = '';
      this.districts = [];
      this.zipCode = '';

      URLBaseAPI.get('/api/CountryDivision/Counties', {
        params: { province: this.selectedProvince },
      })
        .then((response) => {
          this.counties = response.data;
        })
        .catch(console.error);
    },
    getDistricts() {
      this.selectedDistrict = '';
      this.districts = [];
      this.zipCode = '';

      URLBaseAPI.get('/api/CountryDivision/Districts', {
        params: { province: this.selectedProvince, county: this.selectedCounty },
      })
        .then((response) => {
          this.districts = response.data;
        })
        .catch(console.error);
    },
    getZipCode() {
      this.zipCode = '';

      URLBaseAPI.get('/api/CountryDivision/ZipCode', {
        params: {
          province: this.selectedProvince,
          county: this.selectedCounty,
          district: this.selectedDistrict,
        },
      })
        .then((response) => {
          this.zipCode = response.data.value;
        })
        .catch(console.error);
    },
    saveCompany() {
      var self = this;
      var form = self.$el.querySelector('.needs-validation');
      if (!form.checkValidity()) {
        return;
      }

      URLBaseAPI.post('/api/Company', {
        CompanyId: this.companyId,
        CompanyName: this.companyName.trim(),
        AddressDetails: this.addressDetails.trim(),
        ZipCode: this.zipCode,
        Telephone: this.telephone !== '' ? this.telephone : null,
        MaxBenefits: Number(this.maxBenefits || 0),
        PaymentFrequency: this.paymentFrequency,
        PayDay1: this.payDay1,
        PayDay2: this.paymentFrequency === 'Quincenal' ? Number(this.payDay2 || 0) : null,
        CreatedBy: Number(this.$session.user?.userId),
      })
        .then(function () {
          self.toastMessage = 'Empresa creada correctamente';
          self.toastType = 'bg-success';
          self.showToast = true;
          setTimeout(function () {
            self.showToast = false;
            window.location.href = '/';
          }, self.toastTimeout);
        })
        .catch(function (error) {
          var msg =
            error && error.response && error.response.data
              ? error.response.data
              : 'Error inesperado al crear la empresa';
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
    this.initBootstrapValidation();
    this.getProvince();
  },
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