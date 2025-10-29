<template>
  <div class="d-flex flex-column">
    <h1 class="text-center mb-3">Mi perfil</h1>

    <div class="container py-4 flex-fill d-flex justify-content-center">
      <form
        ref="profileForm"
        @submit.prevent="onSubmit"
        class="row g-3 needs-validation"
        novalidate
        style="width: 800px"
      >
        <!-- Datos personales -->
        <h5 class="mt-2">Datos personales</h5>

        <div class="mb-3 col-xl-6 col-sm-12">
          <label for="firstName" class="form-label required">Primer nombre</label>
          <input type="text" class="form-control" id="firstName" required maxlength="60" v-model="model.personData.name1" />
          <div class="invalid-feedback">Ingrese el primer nombre</div>
        </div>

        <div class="mb-3 col-xl-6 col-sm-12">
          <label for="secondName" class="form-label">Segundo nombre</label>
          <input type="text" class="form-control" id="secondName" maxlength="60" v-model="model.personData.name2" />
        </div>

        <div class="mb-3 col-xl-6 col-sm-12">
          <label for="surname1" class="form-label required">Primer apellido</label>
          <input type="text" class="form-control" id="surname1" required maxlength="60" v-model="model.personData.surname1" />
          <div class="invalid-feedback">Ingrese el primer apellido</div>
        </div>

        <div class="mb-3 col-xl-6 col-sm-12">
          <label for="surname2" class="form-label required">Segundo apellido</label>
          <input type="text" class="form-control" id="surname2" required maxlength="60" v-model="model.personData.surname2" />
          <div class="invalid-feedback">Ingrese el segundo apellido</div>
        </div>

        <div class="mb-3 col-xl-4 col-sm-12">
          <label for="idCard" class="form-label">Cédula</label>
          <input type="text" class="form-control disabled" id="idCard" v-model="model.personData.cedula" disabled />
        </div>

        <div class="mb-3 col-xl-4 col-sm-12">
          <label for="phone" class="form-label">Teléfono</label>
          <input type="text" class="form-control" id="phone" placeholder="0000-0000" pattern="[0-9]{4}-[0-9]{4}" v-model="model.personData.phone" />
          <div class="invalid-feedback">Use el formato ####-####</div>
        </div>

        <div class="mb-3 col-xl-4 col-sm-12">
          <label for="birth" class="form-label">Fecha de nacimiento</label>
          <input type="date" class="form-control disabled" id="birth" v-model="birthStr" disabled />
        </div>

        <!-- Dirección -->
        <h5 class="mt-2">Dirección</h5>

        <div class="mb-3 col-xl-6 col-sm-12">
          <label for="province" class="form-label">Provincia</label>
          <select class="form-select" id="province" v-model="Direccion.selectedProvince" @change="getCounties">
            <option selected disabled value="">Seleccione una Provincia</option>
            <option v-for="p in provinces" :key="p.value" :value="p.value">{{ p.value }}</option>
          </select>
        </div>

        <div class="mb-3 col-xl-6 col-sm-12">
          <label for="county" class="form-label">Cantón</label>
          <select class="form-select" id="county" v-model="Direccion.selectedCanton" @change="getDistricts" :disabled="!Direccion.selectedProvince || counties.length === 0">
            <option selected disabled value="">Seleccione un cantón</option>
            <option v-for="c in counties" :key="c.value" :value="c.value">{{ c.value }}</option>
          </select>
        </div>

        <div class="mb-3 col-xl-6 col-sm-12">
          <label for="district" class="form-label">Distrito</label>
          <select class="form-select" id="district" v-model="Direccion.selectedDistrit" @change="getZipCode" :disabled="!Direccion.selectedCanton || districts.length === 0">
            <option selected disabled value="">Seleccione un distrito</option>
            <option v-for="d in districts" :key="d.value" :value="d.value">{{ d.value }}</option>
          </select>
        </div>

        <div class="mb-3 col-xl-6 col-sm-12">
          <label for="zipCode" class="form-label">Código Postal</label>
          <input type="text" class="form-control" id="zipCode" disabled readonly v-model="zipCodeValue" />
        </div>

        <div class="mb-3">
          <label for="otherSigns" class="form-label">Otras señas</label>
          <textarea class="form-control" id="otherSigns" rows="3" maxlength="250" v-model="Direccion.otherSigns"></textarea>
        </div>

        <!-- Pago -->
        <h5 class="mt-2">Pago</h5>
        <div class="mb-3 col-xl-12 col-sm-12">
          <label for="iban" class="form-label">Cuenta bancaria (IBAN CR)</label>
          <input type="text" class="form-control" id="iban" placeholder="CRXXXXXXXXXXXXXXXXXXXX" :pattern="ibanPattern" v-model.trim="model.contractData.bankAccount" />
          <div class="invalid-feedback">Formato IBAN inválido (CR + 20 dígitos)</div>
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
import URLBaseAPI from '../axiosAPIInstances.js'
import { useSession } from '../utils/useSession.js'
import { useGlobalAlert } from '@/utils/alerts.js'
import LinkButton from './LinkButton.vue'

export default {
  name: 'ModifyProfile',
  components: { LinkButton },
  data() {
    return {
      personId: null,
      provinces: [],
      counties: [],
      districts: [],
      Direccion: { selectedProvince: '', selectedCanton: '', selectedDistrit: '', otherSigns: '' },
      model: {
        personData: {
          personID: 0,
          cedula: '',
          name1: '', name2: null,
          surname1: '', surname2: '',
          phone: '',
          birthDate: '',
          personType: 'Empleado',
          company: null
        },
        userData: null,
        contractData: {
          idContract: 0,
          position: '', department: '', salary: 0,
          employeeType: null,
          startDate: null, endDate: null,
          bankAccount: '',
          personID: 0
        },
        companyUniqueId: null,
        direction: {
          idDirection: 0,
          idDivision: 0,
          province: '', canton: '', district: '',
          zipCode: '', otherSigns: null,
          idCompany: null, intPerson: null
        }
      }
    }
  },
  computed: {
    birthStr: {
      get() {
        const v = this.model?.personData?.birthDate
        if (!v) return ''
        const d = new Date(v)
        if (Number.isNaN(d.getTime())) return ''
        return d.toISOString().slice(0, 10)
      },
      //set(_) {}
    },
    //ibanPattern() { return 'CR\d{20}' },
    zipCodeValue() { return this.model?.direction?.zipCode || this.zipCode?.value || this.zipCode || '' }
  },
  methods: {
    initBootstrapValidation() {
      const form = this.$refs.profileForm
      if (!form) return
      form.addEventListener('submit', (event) => {
        if (!form.checkValidity()) { event.preventDefault(); event.stopPropagation() } else { event.preventDefault() }
        form.classList.add('was-validated')
      })
    },
    parseSession() {
      const sess = useSession()
      if (!sess) throw new Error('Sesión inválida: personId no disponible')
      this.personId = sess.user.personId
    },
    async GetProvince() {
      //this.provinces = []
      const { data } = await URLBaseAPI.get('/api/CountryDivision/Provinces')
      this.provinces = data
    },
    async getCounties() {
      this.counties = []
      this.districts = []
      this.model.direction.zipCode = ''
      const { data } = await URLBaseAPI.get('/api/CountryDivision/Counties', { params: { province: this.Direccion.selectedProvince } })
      this.counties = data
    },
    async getDistricts() {
      this.districts = []
      this.model.direction.zipCode = ''
      const { data } = await URLBaseAPI.get('/api/CountryDivision/Districts', { params: { province: this.Direccion.selectedProvince, county: this.Direccion.selectedCanton } })
      this.districts = data
    },
    async getZipCode() {
      const { data } = await URLBaseAPI.get('/api/CountryDivision/ZipCode', { params: { province: this.Direccion.selectedProvince, county: this.Direccion.selectedCanton, district: this.Direccion.selectedDistrit } })
      this.model.direction.zipCode = data?.value ?? data
    },
    async load() {
      try {
        const { data } = await URLBaseAPI.get(`/api/Employee/${this.personId}`)
        // Asignar defensivamente respetando el shape
        this.model.personData = { ...this.model.personData, ...(data?.personData ?? {}) }
        this.model.contractData = { ...this.model.contractData, ...(data?.contractData ?? {}) }
        this.model.direction = { ...this.model.direction, ...(data?.direction ?? {}) }
        this.model.companyUniqueId = data?.companyUniqueId ?? this.model.companyUniqueId
        // Preselección de selects a partir de direction
        this.Direccion.selectedProvince = this.model.direction.province || ''
        await this.getCounties()
        this.Direccion.selectedCanton = this.model.direction.canton || ''
        await this.getDistricts()
        this.Direccion.selectedDistrit = this.model.direction.district || ''
        this.Direccion.otherSigns = this.model.direction.otherSigns || ''
      } catch (err) {
        console.error(err)
        useGlobalAlert().show('No se pudo cargar su información.', 'danger')
      }
    },
    buildPayload() {
      // Aplicar dirección del selector a model.direction si el usuario la tocó
      const touched = this.Direccion.selectedProvince || this.Direccion.selectedCanton || this.Direccion.selectedDistrit || this.Direccion.otherSigns
      let direction = this.model.direction
      if (touched) {
        direction = {
          ...this.model.direction,
          province: this.Direccion.selectedProvince || null,
          canton: this.Direccion.selectedCanton || null,
          district: this.Direccion.selectedDistrit || null,
          otherSigns: this.Direccion.otherSigns || null
        }
      }
      return {
        personData: { ...this.model.personData },
        userData: null,
        contractData: { ...this.model.contractData },
        companyUniqueId: this.model.companyUniqueId,
        direction
      }
    },
    async onSubmit() {
      const form = this.$refs.profileForm
      if (!form.checkValidity()) { form.classList.add('was-validated'); return }
      const payload = this.buildPayload()
      try {
        await URLBaseAPI.put(`/api/Employee/me/${this.personId}`, payload)
        useGlobalAlert().show('Datos actualizados.', 'success')
      } catch (err) {
        console.error(err)
        useGlobalAlert().show('No se pudo actualizar.', 'danger')
      }
    },
    goBack() { window.history.back() }
  },
  async mounted() {
    this.parseSession()
    this.initBootstrapValidation()
    await this.GetProvince()
    await this.load()
  }
}
</script>

<style scoped>
.form-label.required::after { content: ' *'; color: red; }
.disabled { color: #718096; }
.pair-btn { display: flex; gap: 20px; }
</style>
