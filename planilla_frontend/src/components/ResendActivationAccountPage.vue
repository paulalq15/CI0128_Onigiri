<template>
  <div class="d-flex flex-column" id="container">
    <div class="d-flex justify-content-center align-items-center">
      <form
        class="form-signig p-5 h-100 d-inline-block"
        style="max-width: 550px; width: 100%"
        @submit.prevent="saveRegisterData"
      >
        <h2 class="font-weigth-normal text-center mb-4">Reenvío de correo de activación</h2>
        <InputType
          label="Correo electrónico"
          id="email"
          type="email"
          placeHolder="name@example.com"
          v-model="email"
        />
        <br>
        <LinkButton 
          :text="textInButton" 
          :disabled="!email"
          @click="handleClick" 
        />
      </form>
    </div>
  </div>
</template>

<script setup>
  import { ref } from 'vue';
  import { useRouter } from 'vue-router';
  import { useGlobalAlert } from '@/utils/alerts.js';
  import URLBaseAPI from '../axiosAPIInstances.js';
  import InputType from './InputType.vue';
  import LinkButton from './LinkButton.vue';

  var router = useRouter();
  var email = ref('');
  var emailSended = ref(false);
  var textInButton = ref('Enviar correo de activación');

  async function sendEmail() {
    const alert = useGlobalAlert();

    try {
      const response = await URLBaseAPI.post('https://localhost:7071/api/Tokens/ResendActivation', `"${email.value}"`);

      alert.show(response.data.message, 'success');
      emailSended.value = true;
      textInButton.value = 'Ir a iniciar sesión';
    } catch (error) {
      if (error.response) {
        alert.show(error.response.data.message || 'Error del backend', 'warning');
      } else {
        alert.show('Error de red: ' + error.message, 'error');
      }
    }
  }

  function handleClick() {
    if (!emailSended.value) {
      sendEmail();
    } else {
      router.push('/auth/Login');
    }
  }
</script>

<style>
</style>