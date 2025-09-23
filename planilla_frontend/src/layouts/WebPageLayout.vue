<template>
    <div class="d-flex flex-column min-vh-100" id="webpageLayout">
        <HeaderComp />
        <SidebarComp />
        <main class="flex-fill">
            <router-view />
        </main>
        <FooterComp />
    </div>
</template>

<script setup>
    import HeaderComp from '../components/HeaderComp.vue'
    import SidebarComp from '../components/SidebarComp.vue'
    import FooterComp from '../components/FooterComp.vue'
</script>