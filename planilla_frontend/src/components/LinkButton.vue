<template>
  <button class="btn btn-lg btn-block w-100" v-bind:type="type">
    {{ text }}
  </button>
</template>

<script>
export default {
  name:"ButtonLink",
  props: {
    text: {
      type: String,
      required: true
    },

    type: {
      type: String,
      default: "button"
    }
  }
}
</script>

<style lang="scss" scoped>
  button {
    background-color: #1C4532;
    border-color: #1C4532;
    color: white;
  }

  button:hover {
    color: #1C4532;
    border-color: #7A9E70;
    background-color: #7A9E70;
  }

  button:disabled {
    background-color: #6FB595;
    border-color: #6FB595;
    color: #ffffffa0;
    cursor: not-allowed;
    opacity: 0.7;
  }
</style>
