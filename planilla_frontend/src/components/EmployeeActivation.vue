<template>
  <div class="d-flex justify-content-center align-items-center vh-100">
    <div class="card text-center p-5" style="min-width: 600px;">
      <h2 class="card-title mb-4">{{ messageTitle }}</h2>
      <p class="card-text mb-4">{{ messageBody }}</p>
      <LinkButton :text="textInButton" @click="gotToLink" />
    </div>
  </div>
</template>

<script setup>
  import { ref, onMounted } from 'vue'
  import { useRoute, useRouter } from 'vue-router'

  import LinkButton from './LinkButton.vue';

  var textInButton = ref('#');
  var messageTitle = ref('#');
  var messageBody = ref('Ir a Iniciar Sesión');
  var Link = ref('/auth/Login');

  const route = useRoute();
  const router = useRouter();
  const status = route.query.status;

  const bodyCard = {
    success: {
      title: '¡***Cuenta activada!',
      body: 'Su cuenta ha sido activada correctamente. Ya puede iniciar sesión.',
      buttonText: 'Ir a Iniciar Sesión',
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

  onMounted(() => {
    if (status && bodyCard[status]) {
      const cardData = bodyCard[status];
      messageTitle.value = cardData.title;
      messageBody.value = cardData.body;
      textInButton.value = cardData.buttonText;
      Link.value = cardData.buttonLink;
    }
  });

  function gotToLink() {
    router.push(Link.value)
  }
</script>

<style>
.card {
  border: none;
  border-radius: 10px;
  background: none;
}
</style>