<template>
  <div class="form-group">
    <label v-bind:for="id">{{ label }}</label>
    <input
      class="form-control"
      v-bind:type="type"
      v-bind:id="id"
      v-bind:placeholder="placeHolder"
      v-bind:required="required"
    >
  </div>
</template>

<script>
export default {
  name:"InputType",
  props: {
    label: {
      type: String,
      required: true
    },

    id: {
      type: String,
      required: true
    },

    type: {
      type: String,
      default: "text"
    },

    placeHolder: {
      type: String,
      default: ""
    },

    required: {
      type: Boolean,
      default: true
    }
  }
}
</script>

<style lang="scss" scoped>
</style>
