<template>
    <HeaderComp />
    <main>
        <h1>Home Page</h1>
    </main>
    <FooterComp />
</template>

<script>
    import HeaderComp from './HeaderComp.vue';
    import FooterComp from './FooterComp.vue';
    export default {
        components: {
        HeaderComp,
        FooterComp
        }
    }

</script>

<style lang="scss" scoped>

</style>