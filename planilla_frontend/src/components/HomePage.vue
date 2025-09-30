<template>
    <body class="d-flex flex-column min-vh-100">
        <HeaderComp />
        
        <div class="container text-center py-4 flex-fill">
            <div class="row g-5">
                <div class="col-12 col-md-6 col-lg-4">
                    <home-page-card 
                        title="Empresas"
                        icon="bi bi-buildings-fill"
                        :actions="[
                            { label: 'Crear Empresa', icon: 'bi bi-pencil-square', to: '/CrearEmpresa' },
                            { label: 'Ver Empresas', icon: 'bi bi-search', to: '/' }
                        ]"
                    />
                </div>
                <div class="col-12 col-md-6 col-lg-4">
                    <home-page-card 
                        title="Empleados"
                        icon="bi bi-people-fill"
                        :actions="[
                            { label: 'Crear Empleado', icon: 'bi bi-pencil-square', to: '/' },
                            { label: 'Ver Empleados', icon: 'bi bi-search', to: '/' }
                        ]"
                    />
                </div>
                <div class="col-12 col-md-6 col-lg-4">
                    <home-page-card 
                        title="Timesheets"
                        icon="bi bi-calendar-week"
                        :actions="[
                            { label: 'Crear Timesheet', icon: 'bi bi-pencil-square', to: '/' },
                            { label: 'Ver Timesheets', icon: 'bi bi-search', to: '/' }
                        ]"
                    />
                </div>
                <div class="col-12 col-md-6 col-lg-4">
                    <home-page-card 
                        title="Planilla"
                        icon="bi bi-clipboard2-data-fill"
                        :actions="[
                            { label: 'Crear Planilla', icon: 'bi bi-pencil-square', to: '/' },
                            { label: 'Ver Planillas', icon: 'bi bi-search', to: '/' }
                        ]"
                    />
                </div>
                <div class="col-12 col-md-6 col-lg-4">
                    <home-page-card 
                        title="Pagos"
                        icon="bi bi-credit-card-fill"
                        :actions="[
                            { label: 'Crear Pago', icon: 'bi bi-pencil-square', to: '/' },
                            { label: 'Ver Pagos', icon: 'bi bi-search', to: '/' }
                        ]"
                    />
                </div>
                <div class="col-12 col-md-6 col-lg-4">
                    <home-page-card 
                        title="Reportes"
                        icon="bi bi-graph-up"
                        :actions="[
                            { label: 'Crear Reporte', icon: 'bi bi-pencil-square', to: '/' },
                            { label: 'Ver Reportes', icon: 'bi bi-search', to: '/' }
                        ]"
                    />
                </div>
            </div>
        </div>

        <FooterComp />
    </body>
</template>

<script>
    import HeaderComp from './HeaderComp.vue';
    import FooterComp from './FooterComp.vue';
    import HomePageCard from './HomePageCard.vue';
    export default {
        components: {
        HeaderComp,
        FooterComp,
        HomePageCard
        }
    }
</script>

<style lang="scss" scoped>
    
</style>