<template>
  <div class="d-flex flex-column">
    <div class="container mt-5">
      <h3 class="display-5 text-center mb-4">Horas Semanales</h3>
      <div class="d-flex align-items-center gap-2">
        <label class="form-label mb-0">Semana</label>
        <input
          type="date"
          class="form-control"
          style="max-width: 180px"
          v-model="selectedDay"
          @change="onDayChanged"
        />
      </div>
      <div class="card shadow-sm border-0 rounded-3">
        <div class="card-body">
          <div class="row text-center fw-semibold mb-3">
            <div class="col">Lunes</div>
            <div class="col">Martes</div>
            <div class="col">Miércoles</div>
            <div class="col">Jueves</div>
            <div class="col">Viernes</div>
          </div>
          <div class="rounded-3 p-4 bg-light">
            <div class="row text-center align-items-center">
              <div
                class="col"
                v-for="d in weekdaysMonToFri"
                :key="d.key"
              >
                <input
                  type="number"
                  class="form-control text-center fw-semibold"
                  :min="0" :max="9" step="1"
                  v-model.number="hoursModel[d.key]"
                  @input="boundHours(d.key)"
                  :disabled="isLocked(d.date)"
                />
                <div class="form-text mt-1 small text-muted">
                  {{ formatShort(d.date) }}
                </div>
              </div>
            </div>
          </div>
          <div class="mt-4">
            <button
              class="btn btn-success px-4 py-2 rounded-pill fw-semibold"
              @click="confirmar"
              :disabled="loading || !weekStart"
            >
              {{ loading ? 'Guardando...' : 'Confirmar' }}
            </button>
            <span v-if="error" class="text-danger ms-3">{{ error }}</span>
            <span v-if="ok" class="text-success ms-3">Guardado</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { onMounted, reactive, ref, computed } from 'vue'
import URLBaseAPI from '../axiosAPIInstances.js'
import { useSession } from '../utils/useSession.js'

const session = useSession()
const personId = computed(() => session.user?.personId ?? null)

const selectedDay = ref(todayISO())
const weekStart = ref(null)
const weekEnd   = ref(null)

const loading = ref(false)
const error   = ref('')
const ok      = ref(false)

const hoursModel = reactive({})
const descModel  = reactive({})
const lockedMap  = reactive({})

const weekDays = computed(() => {
  if (!weekStart.value) return []
  const days = []
  for (let i = 0; i < 7; i++) {
    const d = addDays(weekStart.value, i)
    days.push({ date: d, key: toISO(d) })
  }
  return days
})

const weekdaysMonToFri = computed(() => weekDays.value.slice(0, 5))

onMounted(() => {
  if (!personId.value) {
    error.value = 'No se encontro el ID del Empleado'
    return
  }
  loadWeekByDay(selectedDay.value)
})

function todayISO () {
  const d = new Date()
  d.setHours(0,0,0,0)
  return d.toISOString().slice(0,10)
}

function toISO (d) {
  const x = new Date(d); x.setHours(0,0,0,0)
  return x.toISOString().slice(0,10)
}

function addDays (d, n) {
  const x = new Date(d)
  x.setDate(x.getDate() + n)
  x.setHours(0,0,0,0)
  return x
}

function formatShort (d) {
  return d.toLocaleDateString('es-CR', { day: '2-digit', month: '2-digit' })
}

async function loadWeekByDay (yyyyMMdd) {
  error.value = ''; ok.value = false; loading.value = true
  try {
    const { data } = await URLBaseAPI.get(`/api/Timesheet/week/${personId.value}`, { params: { day: yyyyMMdd } })

    weekStart.value = new Date(data.weekStart)
    weekEnd.value   = new Date(data.weekEnd)

    for (const d of weekDays.value) {
      hoursModel[d.key] = 0
      lockedMap[d.key]  = false
    }
    if (Array.isArray(data.entries)) {
      for (const e of data.entries) {
        const k = e.date?.slice(0,10)
        if (!k) continue
        hoursModel[k] = Number.isFinite(e.hours) ? e.hours : 0
        lockedMap[k]  = true
      }
    }
  } catch (ex) {
    error.value = 'No se pudieron cargar las horas de la semana.'
  } finally {
    loading.value = false
  }
}

function onDayChanged () {
  if (!selectedDay.value) return
  loadWeekByDay(selectedDay.value)
}

function boundHours (key) {
  let v = Number(hoursModel[key] ?? 0)
  if (isNaN(v)) v = 0
  if (v < 0) v = 0
  if (v > 9) v = 9
  hoursModel[key] = v
}

function isLocked(dateObj) {
  const today = new Date()
  today.setHours(0,0,0,0)

  if (dateObj > today) return true

  const iso = toISO(dateObj)
  return lockedMap[iso] === true
}

async function confirmar () {
  if (!weekStart.value) return
  error.value = ''; ok.value = false; loading.value = true;

  try {
    const weekStartISO = toISO(weekStart.value);
    const today = new Date();
    today.setHours(0,0,0,0);

    const entries = weekdaysMonToFri.value
     .filter(d => d.date <= today)
     .filter(d => !isLocked(d.date))
     .map(d => {
       const hrs  = Number(hoursModel[d.key] ?? 0)
       const desc = (descModel[d.key] ?? '').trim()
       return { date: d.key, hours: hrs, description: desc || null }
     })
     .filter(e => (Number.isFinite(e.hours) && e.hours > 0) || (e.description && e.description.length > 0))

    if (entries.length === 0) {
      error.value = 'No hay días válidos para guardar (no puede registrar días futuros).';
      loading.value = false;
      return;
    }

    await URLBaseAPI.post(`/api/Timesheet/week/${personId.value}`, {weekStart: weekStartISO, entries});

    ok.value = true;
  } catch (ex) {
    error.value = 'No se pudieron guardar las horas.';
  } finally {
    loading.value = false;
    setTimeout(() => (ok.value = false), 2500);
  }
}
</script>

<style lang="scss" scoped></style>