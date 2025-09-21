<template>
    <div class="card mx-auto" style="max-width: 50vw;">
        <div class="card-body">
            <h5 class="card-title mb-3">
            <i :class="icon"></i> {{ title }}
            </h5>

            <div class="d-flex flex-wrap justify-content-center gap-3">
            <router-link
                v-for="action in actions"
                :key="action.label"
                :to="action.to"
                class="p-2 border rounded bg-light d-flex flex-column text-dark text-decoration-none"
                style="width: 10rem;"
            >
                <i :class="action.icon"></i> {{ action.label }}
            </router-link>
            </div>
        </div>
    </div>
</template>

<script>
    export default {
        name: 'HomePageCard',
        props: {
            title: { type: String, required: true },
            icon: { type: String, required: true },
            actions: { type: Array, required: true }
        }
    }
</script>

<style lang="scss" scoped>
</style>