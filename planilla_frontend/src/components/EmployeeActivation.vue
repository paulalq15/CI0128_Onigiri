<template>
  <div class="d-flex justify-content-center align-items-center vh-100">
    <div class="card text-center p-5" style="min-width: 600px;">
      <h2 class="card-title mb-4">{{ messageTitle }}</h2>
      <p class="card-text mb-4">{{ messageBody }}</p>

      <!-- Formulario de contraseña sólo cuando status=success-->
      <div v-if="showPasswordForm" class="text-start mx-auto" style="max-width: 420px;">
        <form
          class="p-5 h-100 d-inline-block"
          style="max-width: 550px; width: 100%"
          @submit.prevent="savePassword"
          novalidate
        >
          <!-- Contraseña -->
          <InputType
            label="Contraseña"
            id="pwd"
            type="password"
            :min-length="8"
            :max-length="16"
            autocomplete="new-password"
            v-model="password"
          />
          <span class="text-danger" v-if="errors.password">{{ errors.password }}</span>

          <!-- Confirmar contraseña -->
          <InputType
            label="Confirmar contraseña"
            id="confPwd"
            type="password"
            :min-length="8"
            :max-length="16"
            autocomplete="new-password"
            v-model="confPassword"
          />
          <span class="text-danger" v-if="errors.confPassword">{{ errors.confPassword }}</span>

          <!-- Mensajes -->
          <div v-if="errorMsg" class="text-danger mb-2">{{ errorMsg }}</div>
          <div v-if="successMsg" class="text-success mb-2">{{ successMsg }}</div>

        </form>
        <hr class="my-4" />
      </div>

      <!-- Botón externo (navegación) -->
      <LinkButton :text="textInButton" @click="savePassword" />
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import LinkButton from './LinkButton.vue'
import InputType from './InputType.vue'

const route = useRoute()
const router = useRouter()

// UI: textos del card y botón
const textInButton = ref('#')
const messageTitle = ref('#')
const messageBody  = ref('') // ← estaba cambiada con el texto del botón
const link         = ref('/auth/Login')

// Estado del flujo/URL
const status = computed(() => (route.query.status ?? '').toString().toLowerCase().trim())
const showPasswordForm = computed(() => status.value === 'success')

// Form: campos, errores y flags (¡inicializados!)
const password     = ref('')
const confPassword = ref('')
const errors = reactive({
  password: '',
  confPassword: ''
})
const errorMsg   = ref('')
const successMsg = ref('')
const saving     = ref(false)

const bodyCard = {
  success: {
    title: '¡Cuenta activada!',
    body: 'Su cuenta ha sido activada correctamente. Defina su contraseña para continuar.',
    buttonText: 'Guardar',
    buttonLink: '/auth/Login'
  },
  failed: {
    title: '¡Error! X',
    body: 'No se pudo activar la cuenta. Por favor comuníquese con soporte.',
    buttonText: 'Ir a Soporte',
    buttonLink: '#'
  },
  expired: {
    title: 'Token expirado X',
    body: 'El enlace de activación ha expirado. Solicite uno nuevo.',
    buttonText: 'Solicitar nuevo enlace de activación',
    buttonLink: '/auth/ResendActivationAccount'
  },
  'invalid-token': {
    title: 'Token inválido X',
    body: 'El enlace de activación no es válido.',
    buttonText: 'Solicitar nuevo enlace de activación',
    buttonLink: '/auth/ResendActivationAccount'
  },
  'invalid-user': {
    title: '¡Error! X',
    body: 'Usuario inválido. Por favor comuníquese con soporte.',
    buttonText: 'Ir a Soporte',
    buttonLink: '#'
  },
  'already-active': {
    title: '¡Cuenta ya activa!',
    body: 'Su cuenta ya ha sido previamente activada. Ya puede iniciar sesión.',
    buttonText: 'Ir a Iniciar Sesión',
    buttonLink: '/auth/Login'
  }
}

// Sin "this": funciones puras Composition API
function hydrateFromStatus() {
  const s = status.value
  if (s && bodyCard[s]) {
    const card = bodyCard[s]
    messageTitle.value = card.title
    messageBody.value  = card.body
    textInButton.value = card.buttonText
    link.value         = card.buttonLink
  } else {
    // fallback
    messageTitle.value = 'Activación'
    messageBody.value  = 'Revise el enlace recibido.'
    textInButton.value = 'Ir a Iniciar Sesión'
    link.value         = '/auth/Login'
  }
}

function validateForm() {
  // limpiar errores
  errors.password = ''
  errors.confPassword = ''
  errorMsg.value = ''

  // Reglas
  if (!password.value) {
    errors.password = 'La contraseña es obligatoria'
  } else if (password.value.length < 8) {
    errors.password = 'La contraseña debe tener al menos 8 caracteres'
  } else {
    if (!/[A-Z]/.test(password.value))
      errors.password = 'La contraseña debe tener al menos una letra mayúscula'
    if (!/[a-z]/.test(password.value))
      errors.password = 'La contraseña debe tener al menos una letra minúscula'
    if (!/[0-9]/.test(password.value))
      errors.password = 'La contraseña debe tener al menos un número'
    if (!/[!@#$%^&*(),.?":{}|<>]/.test(password.value))
      errors.password = 'La contraseña debe tener al menos un carácter especial'
  }

  if (confPassword.value !== password.value) {
    errors.confPassword = 'Las contraseñas no coinciden'
  }

  return !errors.password && !errors.confPassword
}

async function savePassword() {
  if (!validateForm()) return
  try {
    saving.value = true
    // Ejemplo de llamada (ajusta URL, headers y payload a tu backend):
    // await axios.post('/api/users/set-password', { password: password.value }, { params: { token: route.query.token } })

    successMsg.value = 'Contraseña actualizada.'
    errorMsg.value = ''

    router.push(link.value) // puede ser string directo

  } catch (e) {
    successMsg.value = ''
    errorMsg.value = 'No se pudo actualizar la contraseña.'
  } finally {
    saving.value = false
  }
}

// Inicializa en mount y reacciona si cambia el query
onMounted(hydrateFromStatus)
watch(status, hydrateFromStatus)
</script>

<style>
.card {
  border: none;
  border-radius: 10px;
  background: none;
}
</style>