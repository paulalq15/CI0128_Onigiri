<template>
  <div class="d-flex flex-column">
    <h1 class="text-center">Modificar empleado</h1>

    <div class="container py-4 flex-fill d-flex justify-content-center">
      <form
        ref="employeeForm"
        @submit.prevent="onSubmit"
        class="row g-3 needs-validation"
        novalidate
        style="width: 900px"
      >
        <!-- Selector empleado -->
        <div class="mb-3 col-12 d-flex align-items-end gap-2">
          <div class="flex-grow-1">
            <label for="pId" class="form-label required">Id Persona</label>
            <input type="number" class="form-control" id="pId" required v-model.number="targetPersonId" />
            <div class="invalid-feedback">Ingrese el IdPersona</div>
          </div>
          <div class="mb-3">
            <LinkButton @click="loadById" text="Cargar" />
          </div>
        </div>

        <!-- Datos personales -->
        <h5 class="mt-2">Datos personales</h5>
        <div class="mb-3 col-xl-6 col-sm-12">
          <label for="firstNameE" class="form-label required">Primer nombre</label>
          <input type="text" class="form-control" id="firstNameE" required maxlength="60" v-model="model.personData.name1" />
          <div class="invalid-feedback">Ingrese el primer nombre</div>
        </div>
        <div class="mb-3 col-xl-6 col-sm-12">
          <label for="secondNameE" class="form-label">Segundo nombre</label>
          <input type="text" class="form-control" id="secondNameE" maxlength="60" v-model="model.personData.name2" />
        </div>
        <div class="mb-3 col-xl-6 col-sm-12">
          <label for="surname1E" class="form-label required">Primer apellido</label>
          <input type="text" class="form-control" id="surname1E" required maxlength="60" v-model="model.personData.surname1" />
          <div class="invalid-feedback">Ingrese el primer apellido</div>
        </div>
        <div class="mb-3 col-xl-6 col-sm-12">
          <label for="surname2E" class="form-label required">Segundo apellido</label>
          <input type="text" class="form-control" id="surname2E" required maxlength="60" v-model="model.personData.surname2" />
          <div class="invalid-feedback">Ingrese el segundo apellido</div>
        </div>
        <div class="mb-3 col-xl-4 col-sm-12">
          <label for="phoneE" class="form-label">Teléfono</label>
          <input type="text" class="form-control" id="phoneE" placeholder="0000-0000" pattern="[0-9]{4}-[0-9]{4}" v-model="model.personData.phone" />
          <div class="invalid-feedback">Use el formato ####-####</div>
        </div>

        <!-- Dirección -->
        <h5 class="mt-2">Dirección</h5>
        <div class="mb-3 col-xl-6 col-sm-12">
          <label for="provinceE" class="form-label">Provincia</label>
          <select class="form-select" id="provinceE" v-model="Direccion.selectedProvince" @change="getCounties">
            <option selected disabled value="">Seleccione una Provincia</option>
            <option v-for="p in provinces" :key="p.value" :value="p.value">{{ p.value }}</option>
          </select>
        </div>
        <div class="mb-3 col-xl-6 col-sm-12">
          <label for="countyE" class="form-label">Cantón</label>
          <select class="form-select" id="countyE" v-model="Direccion.selectedCanton" @change="getDistricts" :disabled="!Direccion.selectedProvince || counties.length === 0">
            <option selected disabled value="">Seleccione un cantón</option>
            <option v-for="c in counties" :key="c.value" :value="c.value">{{ c.value }}</option>
          </select>
        </div>
        <div class="mb-3 col-xl-6 col-sm-12">
          <label for="districtE" class="form-label">Distrito</label>
          <select class="form-select" id="districtE" v-model="Direccion.selectedDistrit" @change="getZipCode" :disabled="!Direccion.selectedCanton || districts.length === 0">
            <option selected disabled value="">Seleccione un distrito</option>
            <option v-for="d in districts" :key="d.value" :value="d.value">{{ d.value }}</option>
          </select>
        </div>
        <div class="mb-3 col-xl-6 col-sm-12">
          <label for="zipCodeE" class="form-label">Código Postal</label>
          <input type="text" class="form-control" id="zipCodeE" disabled readonly v-model="zipCodeValue" />
        </div>
        <div class="mb-3">
          <label for="otherSignsE" class="form-label">Otras señas</label>
          <textarea class="form-control" id="otherSignsE" rows="3" maxlength="250" v-model="Direccion.otherSigns"></textarea>
        </div>

        <!-- Contrato -->
        <h5 class="mt-2">Contrato</h5>
        <div class="mb-3 col-xl-6 col-sm-12">
          <label for="position" class="form-label">Puesto</label>
          <input type="text" class="form-control" id="position" maxlength="80" v-model="model.contractData.position" />
        </div>
        <div class="mb-3 col-xl-6 col-sm-12">
          <label for="department" class="form-label">Departamento</label>
          <input type="text" class="form-control" id="department" maxlength="80" v-model="model.contractData.department" />
        </div>
        <div class="mb-3 col-xl-6 col-sm-12">
          <label for="salary" class="form-label">Salario</label>
          <input type="number" class="form-control" id="salary" :min="0" step="0.01" v-model.number="model.contractData.salary" />
          <div class="invalid-feedback">Ingrese un salario válido (≥ 0)</div>
        </div>
        <div class="mb-3 col-xl-6 col-sm-12">
          <label for="iban2" class="form-label">Cuenta bancaria (IBAN CR)</label>
          <input type="text" class="form-control" id="iban2" :pattern="ibanPattern" v-model.trim="model.contractData.bankAccount" />
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

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import URLBaseAPI from '../axiosAPIInstances.js'
import { useSession } from '../utils/useSession.js'
import { useGlobalAlert } from '@/utils/alerts.js'
import LinkButton from './LinkButton.vue' // si lo usas en el template

// ---------- refs / state ----------
const route = useRoute()

const employerId = ref(null)
const targetPersonId = ref('')

const provinces = ref([])   // <- usa estos nombres en el template
const counties  = ref([])
const districts = ref([])

const Direccion = reactive({
  selectedProvince: '',
  selectedCanton: '',
  selectedDistrit: '',
  otherSigns: '',
})

const model = reactive({
  personData: {
    personID: 0,
    cedula: '',
    name1: '', name2: null,
    surname1: '', surname2: '',
    phone: '',
    birthDate: null,
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
})

// Para validación Bootstrap
const employeeForm = ref(null)

// ---------- computed ----------
const ibanPattern = computed(() => 'CR\\d{20}')
const zipCodeValue = computed(() =>
  model?.direction?.zipCode ?? ''
)

// ---------- helpers ----------
function initBootstrapValidation() {
  const form = employeeForm.value
  if (!form) return
  form.addEventListener('submit', (event) => {
    if (!form.checkValidity()) { event.preventDefault(); event.stopPropagation() }
    else { event.preventDefault() }
    form.classList.add('was-validated')
  })
}

function parseSession() {
  const { user } = useSession()
  if (!user) throw new Error('Sesión inválida')
  // normalmente employerId = userId del empleador
  employerId.value = user.userId ?? user.personId ?? null
}

// Normaliza a { value: 'Texto' }
const toValueArray = (arr, keyCandidates = []) => {
  return (Array.isArray(arr) ? arr : []).map(x => {
    if (typeof x === 'string') return { value: x }
    for (const k of keyCandidates) {
      if (x?.[k]) return { value: x[k] }
    }
    return { value: String(x ?? '') }
  }).filter(x => x.value)
}

// ---------- API calls ----------
async function GetProvince() {
  const { data } = await URLBaseAPI.get('/api/CountryDivision/Provinces')
  provinces.value = toValueArray(data, ['value', 'provincia', 'Province'])
}

async function getCounties() {
  counties.value = []
  districts.value = []
  model.direction.zipCode = ''
  const { data } = await URLBaseAPI.get('/api/CountryDivision/Counties', {
    params: { province: Direccion.selectedProvince }
  })
  counties.value = toValueArray(data, ['value', 'canton', 'County'])
}

async function getDistricts() {
  districts.value = []
  model.direction.zipCode = ''
  const { data } = await URLBaseAPI.get('/api/CountryDivision/Districts', {
    params: { province: Direccion.selectedProvince, county: Direccion.selectedCanton }
  })
  districts.value = toValueArray(data, ['value', 'district', 'Distrito'])
}

async function getZipCode() {
  const { data } = await URLBaseAPI.get('/api/CountryDivision/ZipCode', {
    params: {
      province: Direccion.selectedProvince,
      county: Direccion.selectedCanton,
      district: Direccion.selectedDistrit
    }
  })
  model.direction.zipCode = data?.value ?? data
}

// ---------- hydrate / load ----------
async function hydrateAddress(dr) {
  await GetProvince()
  if (!dr) {
    Direccion.selectedProvince = ''
    Direccion.selectedCanton = ''
    Direccion.selectedDistrit = ''
    Direccion.otherSigns = ''
    return
  }
  Direccion.selectedProvince = dr.province || ''
  await getCounties()
  Direccion.selectedCanton = dr.canton || ''
  await getDistricts()
  Direccion.selectedDistrit = dr.district || ''
  Direccion.otherSigns = dr.otherSigns || ''
}

async function loadById() {
  if (!targetPersonId.value) return
  try {
    const { data } = await URLBaseAPI.get(`/api/Employee/${targetPersonId.value}`)
    Object.assign(model.personData,   data?.personData   ?? {})
    Object.assign(model.contractData, data?.contractData ?? {})
    Object.assign(model.direction,    data?.direction    ?? {})
    model.companyUniqueId = data?.companyUniqueId ?? model.companyUniqueId
    await hydrateAddress(model.direction)
  } catch (e) {
    console.error(e)
    useGlobalAlert().show('No se encontró el empleado.', 'warning')
  }
}

// ---------- submit ----------
function buildPayload() {
  const hasDir =
    Direccion.selectedProvince ||
    Direccion.selectedCanton ||
    Direccion.selectedDistrit ||
    Direccion.otherSigns

  let direction = model.direction
  if (hasDir) {
    direction = {
      ...model.direction,
      province: Direccion.selectedProvince || null,
      canton: Direccion.selectedCanton || null,
      district: Direccion.selectedDistrit || null,
      otherSigns: Direccion.otherSigns || null
    }
  }

  return {
    personData: { ...model.personData },
    userData: null,
    contractData: { ...model.contractData },
    direction
  }
}

async function onSubmit() {
  const form = employeeForm.value
  if (!form?.checkValidity()) { form?.classList.add('was-validated'); return }
  if (!targetPersonId.value) { useGlobalAlert().show('Indique IdPersona y presione Cargar', 'warning'); return }
  const payload = buildPayload()
  try {
    await URLBaseAPI.put(`/api/Employee/emp/${employerId.value}/employee/${targetPersonId.value}`, payload)
    useGlobalAlert().show('Empleado actualizado.', 'success')
  } catch (err) {
    console.error(err)
    useGlobalAlert().show('No se pudo actualizar el empleado.', 'danger')
  }
}

function goBack() {
  window.history.back()
}

// ---------- lifecycle ----------
onMounted(async () => {
  const p = route.query?.personId
  if (p !== undefined) {
    targetPersonId.value = Number(p)
    await loadById()
  }
  parseSession()
  initBootstrapValidation()
  await GetProvince()
})
</script>

<style scoped>
.form-label.required::after { content: ' *'; color: red; }
.disabled { color: #718096; }
.pair-btn { display: flex; gap: 20px; }
</style>
