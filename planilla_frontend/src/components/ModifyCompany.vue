<template>
  <div class="d-flex flex-column">
    <h1 class="text-center">Editar Empresa - {{ companyNameTitle }}</h1>

    <div class="container py-4 flex-fill d-flex justify-content-center">
      <form
        ref="companyForm"
        @submit.prevent="UpdateCompanyData"
        class="row g-3 needs-validation"
        novalidate
        style="width: 800px"
      >
        <div class="mb-3 col-xl-3 col-sm-12">
          <label for="companyId" class="form-label">Cédula Jurídica</label>
          <input
            type="text"
            class="form-control disabled"
            id="companyId"
            pattern="[0-9]{1}-[0-9]{3}-[0-9]{6}"
            v-model="companyId"
            disabled
          />
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
            :min="minBenefits"
            required
            v-model.number="maxBenefits"
          />
          <div class="invalid-feedback">
            Ingrese la cantidad máxima de beneficios que puede seleccionar un empleado
          </div>
        </div>

        <!--Payment frequency and days-->
        <div class="mb-3">
          <label for="paymentFrequency" class="form-label">Frecuencia de pago</label>
          <input
            type="text"
            class="form-control disabled"
            id="paymentFrequency"
            v-model="paymentFrequency"
            disabled
          />
        </div>

        <div class="mb-3">
          <label for="payDay1" class="form-label">Día de pago</label>
          <input
            type="text"
            class="form-control disabled"
            id="payDay1"
            v-model="payDay1"
            disabled
          />
        </div>

        <div v-if="paymentFrequency === 'Quincenal'" class="mb-3">
          <label for="payDay2" class="form-label">Segundo día de pago</label>
          <input
            type="text"
            class="form-control disabled"
            id="payDay2"
            v-model="payDay2"
            disabled
          />
        </div>

        <div class="mb-3 pair-btn">
          <LinkButton @click="goBack" text="Cancelar" />
          <LinkButton text="Guardar" type="submit" />
        </div>

      </form>
    </div>
  </div>
</template>

<script>
  import URLBaseAPI from '../axiosAPIInstances.js';
  import { useSession } from '@/utils/useSession';
  import { useGlobalAlert } from '@/utils/alerts.js';
  import LinkButton from './LinkButton.vue';

  export default {
    components: {
    LinkButton,
    },

    data() {
      return {
        companyUniqueId: 0,
        user: null,
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
        companyNameTitle: '',
        addressDetails: '',
        telephone: '',
        maxBenefits: '',
        minBenefits: 0,
      };
    },

    methods: {
      initBootstrapValidation() {
        const form = this.$refs.companyForm;
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

      async InitFormData(companyId) {
        try {
          const response = await URLBaseAPI.get('/api/Company/GCBCUI', { params: { companyUniqueId: companyId } });
          const company = response.data;

          this.companyId = company.companyId;
          this.companyName = company.companyName;
          this.companyNameTitle = this.companyName;
          this.telephone = company.telephone || '';
          this.maxBenefits = company.maxBenefits;
          this.paymentFrequency = company.paymentFrequency;
          this.payDay1 = company.payDay1;
          this.payDay2 = company.payDay2 || '';

          this.selectedProvince = company.directions.province;
          await this.getCounties();

          this.selectedCounty = company.directions.canton;
          await this.getDistricts();

          this.selectedDistrict = company.directions.district;
          this.zipCode = company.directions.zipCode;
          this.addressDetails = company.directions.otherSigns || '';
        } catch (error) {
          console.error('Error al cargar los datos de la empresa:', error);
        }
      },

      async setMinBenefits(companyId) {
        try {
          const response = await URLBaseAPI.get('/api/Company/MaxBenTak', { params: { companyUniqueId: companyId } });
          this.minBenefits = response.data;
          console.log(this.minBenefits + " y " + response.data);
        } catch (error) {
          console.error('Error al cargar los datos de la empresa:', error);
        }

      },

      getProvince() {
        URLBaseAPI.get('/api/CountryDivision/Provinces').then((response) => {
          this.provinces = response.data;
        });
      },

      getCounties() {
        this.counties = [];
        this.districts = [];
        this.zipCode = '';

        return URLBaseAPI.get('/api/CountryDivision/Counties', {
            params: { province: this.selectedProvince },
        })
        .then(response => { this.counties = response.data; })
        .catch(console.error);
      },

      getDistricts() {
        this.districts = [];
        this.zipCode = '';

        return URLBaseAPI.get('/api/CountryDivision/Districts', {
            params: { province: this.selectedProvince, county: this.selectedCounty },
        })
        .then(response => { this.districts = response.data; })
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

      UpdateCompanyData() {
        const alert = useGlobalAlert();

        let company = {
          companyUniqueId: this.companyUniqueId,
          companyId: this.companyId,
          companyName: this.companyName,
          Telephone: this.telephone,
          MaxBenefits: this.maxBenefits,
          paymentFrequency: this.paymentFrequency,
          directions: {
            province: this.selectedProvince,
            canton: this.selectedCounty,
            district: this.selectedDistrict,
            zipCode: this.zipCode,
            otherSigns: this.addressDetails
          }
        }

        URLBaseAPI.post('/api/Company/UpdComp', company)
            .then((response) => {
              if (response.data > 0) {
                alert.show('¡Datos de la empresa actualizados correctamente!', 'success');
                // Recargar la página completa
                window.location.reload();
              } else {
                alert.show('No se pudo actualizar los datos de la empresa, intente más tarde. \nSi el problema persiste, comuníquese con soporte', 'warning');
              }
            })

      },

      goBack() {
        window.history.back();
      }
    },

    async mounted() { 
      const { user } = useSession();
      this.user = user;

      if (!this.user || this.user.typeUser !== 'Empleador') return;

      this.companyUniqueId = user.companyUniqueId;
      await this.InitFormData(this.companyUniqueId);

      await this.setMinBenefits(this.companyUniqueId);

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

.disabled {
  color: #718096;
}

.pair-btn {
  display: flex;
  gap: 20px;
}
</style>