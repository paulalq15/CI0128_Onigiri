<template>
  <div class="pop-up">
    <div class="pop-up-inner">
      <div class="pop-up-close" @click="closePopUp">
        &times;
      </div>

      <div class="pop-up-content">
        <h1>{{ mainText }}</h1>
        <p>{{ bodyText }}</p>

        <div class="pair-btn">
          <LinkButton :text="fistButtontext" @click="accept" />
          <LinkButton :text="secondButtontext" @click="cancel" />
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import LinkButton from '../LinkButton.vue';

export default {
  name: "PopUp",

  components: {
    LinkButton,
  },

  props: {
    mainText: {
      type: String,
      default: 'Acá va el texto principal',
    },
    bodyText: {
      type: String,
      default: 'Acá va el texto secundario',
    },
    fistButtontext: {
      type: String,
      default: 'Botón 1',
    },
    secondButtontext: {
      type: String,
      default: 'Botón 2',
    }
  },

  methods: {
    closePopUp() {
      this.$emit('closePopUp');
    },

    accept() {
      this.$emit('closePopUp');
      this.$emit('resolved', true);
    },

    cancel() {
      this.$emit('closePopUp');
      this.$emit('resolved', false);
    }
  }
};
</script>

<style lang="scss">
.pop-up {
  position: fixed;
  top: 0;
  left: 0;
  z-index: 10;
  padding: 32px 16px 120px;
  width: 100%;
  height: 100vh;
  background-color: rgba(0, 0, 0, 0.5);
  display: grid;
  place-items: center;

  &-close {
    position: absolute;
    height: 52px;
    width: 52px;

    display: flex;
    justify-content: center;
    align-items: center;
    top: 0;
    right: 0;
    font-size: 3rem;
    color: #d4d4d4;
    cursor: pointer;
  }

  &-inner {
    background-color: white;
    color: #000;
    position: relative;
    width: 60%;
    padding: 40px;
    border-radius: 10px;
    box-shadow: 0 5px 5px rgba(0, 0, 0, 0.1);
    transition: all 250ms ease-in-out;
  }
}

.pop-up-content {
  text-align: center;
  width: 100%;
}

.fade-enter, .fade-leave-to {
  opacity: 0;

  .pop-up-inner {
    opacity: 0;
    transform: translateY(-32px);
  }
}

.fade-enter-active, .fade-leave-active {
  transition: all 250ms ease-in-out;
}

.fade-leave-active {
  transition-delay: 250ms;
}

.pair-btn {
  display: flex;
  gap: 20px;
}
</style>
