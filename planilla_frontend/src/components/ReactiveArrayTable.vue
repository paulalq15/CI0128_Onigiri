<template>
  <div class="table-responsive pt-3 pb-3 ps-4 pe-4" style="background-color: white; border-radius: 10px;">
    <table class="table">
      <thead>
        <tr>
          <th scope="col">#</th>
          <th v-for="(head, index) in tableHeader" :key="index" scope="col">
            {{ head.label }}
          </th>
        </tr>
      </thead>
      <tbody>
        <template v-for="(element, index) in tableElements" :key="index">
          <tr>
            <td>{{ index + 1 }}</td>
            <td v-for="(head, j) in tableHeader" :key="j">
              <span v-if="head.key === 'action'" v-html="element[head.key]" @click="handleActionClick"></span>
              <span v-else>{{ element[head.key] }}</span>
            </td>
          </tr>
          <tr v-if="index < tableElements.length - 1" class="spacer">
            <td :colspan="tableHeader.length + 1"></td>
          </tr>
        </template>
      </tbody>
    </table>
  </div>
</template>

<script>
  export default {
    name: "ReactiveObjectTable",
    props: {
      tableHeader: { type: Array, required: true },
      tableElements: { type: Array, required: true }
    },
    emits: ['action'],
    methods: {
      handleActionClick(event) {
        //const id = event.target.dataset.id;
        //if (id) this.$emit('action', id);
        const button = event.target.closest('button[data-id]');
        if (!button) return;

        const id = button.dataset.id;
        const action = button.dataset.action || null; // "edit" o "delete"

        if (id && action) this.$emit('action', { id, action });
      }
    }
  };
</script>


<style scoped>
  table.table td {
    border: none;
  }
</style>
