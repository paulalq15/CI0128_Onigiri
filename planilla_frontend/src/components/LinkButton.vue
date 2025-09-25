<template>
  <button class="btn btn-lg btn-primary btn-block w-100" type="submit">
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
    }
  }
}
</script>

<style lang="scss" scoped>
  #container > div > form > div.form-group.text-center > button  {
    background-color: #1C4532;
    border-color: #1C4532;
  }

  #container > div > form > div.form-group.text-center > button:hover {
    color: #1C4532;
    border-color: #6FB595;
    background-color: #6FB595;
  }
</style>
