<template>
  <div class="d-flex flex-column" id="container">
    <!--Header Onigiri Solutions-->
    <HeaderOnigiri />
    <!--Cuerpo-->
    <div class="d-flex justify-content-center align-items-center">
      <!--Formulario-->
      <form class="form-signig p-5 h-100 d-inline-block" style="max-width: 550px; width: 100%;"  @submit.prevent="saveRegisterData">
        <h2 class="font-weigth-normal text-center mb-4">Registrarse como Empleador</h2>
        <!--Primer nombre-->
        <InputType label="Primer nombre" id="firstName" v-model="Persona.Name1" />
        <span class="text-danger" v-if="errors.Name1">{{ errors.Name1 }}</span>
        <!--Segundo nombre-->
        <InputType label="Segundo nombre" id="SecondName" v-model="Persona.Name2" :required="false"/>
        <!--Primer apellido-->
        <InputType label="Primer apellido" id="firstSurname" v-model="Persona.Surname1" />
        <span class="text-danger" v-if="errors.Surname1">{{ errors.Surname1 }}</span>
        <!--Segundo apellido-->
        <InputType label="Segundo apellido" id="secondSurname" v-model="Persona.Surname2" />
        <span class="text-danger" v-if="errors.Surname2">{{ errors.Surname2 }}</span>
        <!--Cédula-->
        <InputType label="Cédula de indentidad" id="IDCard" placeHolder="0-0000-0000" :max_length="11" v-model="Persona.IdCard" />
        <span class="text-danger" v-if="errors.IdCard">{{ errors.IdCard }}</span>
        <!--Número telefónico-->
        <InputType label="Número de teléfono" id="cellphone" type="tel" placeHolder="0000-0000" :max_length="9" v-model="Persona.Number" />
        <span class="text-danger" v-if="errors.Number">{{ errors.Number }}</span>
        <!--Dirección: Provincia-->
        <div class="form-group mt-3">
          <label for="province">Provincia<span style="color: red;"> *</span></label>
          <select v-model="Direccion.selectedProvince" class="form-control" id="province" required @change="getCounties">
            <option value="" disabled selected>Seleccione una provincia</option>
            <option v-for="prov in provincias" :key="prov.value" :value="prov.value"> {{ prov.value }} </option>
          </select>
        </div>
        <span class="text-danger" v-if="errors.Provincia">{{ errors.Provincia }}</span>
        <!--Dirección: Cantón-->
        <div class="form-group mt-3">
          <label for="canton">Cantón<span style="color: red;"> *</span></label>
          <select v-model="Direccion.selectedCanton" class="form-control" id="canton" :disabled="!Direccion.selectedProvince" required @change="getDistricts">
            <option value="" disabled selected>Seleccione un cantón</option>
            <option v-for="canton in cantones" :key="canton.value" :value="canton.value"> {{ canton.value }} </option>
          </select>
        </div>
        <span class="text-danger" v-if="errors.Canton">{{ errors.Canton }}</span>
        <!--Dirección: Distrito-->
        <div class="form-group mt-3">
          <label for="distrit">Distrito<span style="color: red;"> *</span></label>
          <select v-model="Direccion.selectedDistrit" class="form-control" id="distrit" :disabled="!Direccion.selectedCanton" required @change="getZipCode">
            <option value="" disabled selected>Seleccione un distrito</option>
            <option v-for="distrit in distritos" :key="distrit.value" :value="distrit.value"> {{ distrit.value }} </option>
          </select>
        </div>
        <span class="text-danger" v-if="errors.Distrito">{{ errors.Distrito }}</span>
        <!--Dirección: Otras señas-->
        <div class="form-group mt-3">
          <label for="otherSigns">Otras señas<span style="color: red;"> *</span></label>
          <textarea class="form-control" style="max-height: 200px;" id="otherSigns" rows="3" maxlength="200" v-model="Direccion.otherSigns" required></textarea>
        </div>
        <span class="text-danger" v-if="errors.otherSigns">{{ errors.otherSigns }}</span>
        <!--Fecha nacimiento-->
        <InputType label="Fecha de nacimiento" id="birthday" type="date" v-model="Persona.BirthdayDate" />
        <span class="text-danger" v-if="errors.BirthdayDate">{{ errors.BirthdayDate }}</span>
        <!--Correo electrónico-->
        <InputType label="Correo electrónico" id="email" type="email" placeHolder="name@example.com" v-model="Persona.Email" />
        <span class="text-danger" v-if="errors.Email">{{ errors.Email }}</span>
        <!--Contraseña-->
        <InputType label="Contraseña" id="pwd" type="password" :min_length="8" :max_length="16" v-model="password" />
        <span class="text-danger" v-if="errors.Password">{{ errors.Password }}</span>
        <!--Confirmar contraseña-->
        <InputType label="Confirmar contraseña" id="confPwd" type="password" :min_length="8" :max_length="16" v-model="confPassword" />
        <span class="text-danger" v-if="errors.ConfPassword">{{ errors.ConfPassword }}</span>
        <!--Términos y condiciones e iniciar sesión-->
        <div class="d-flex flex-column justify-content-center align-items-center mt-4 mb-4 text-center" id="termsandlogin">
          <p class="mb-4" style="font-size: small;">
            Al continuar, aceptas nuestros 
            <a href="#" style="color: blue;">Términos de servicio</a> y 
            <a href="#">Política de privacidad</a>
          </p>
          <div>
            <span class="me-2">¿Ya tienes una cuenta?</span>
            <a href="#">Inicie Sesión</a>
          </div>
        </div>

        <!--Botón confirmar-->
        <div class="form-group text-center">
          <LinkButton type="button" @click="saveRegisterData" text="Registrarse" />
        </div>
      </form>
    </div>
    <FooterComp />
  </div>
</template>

<script>
  // Alerta global en App.vue
  import { useGlobalAlert } from '@/utils/alerts.js'

  import { toRaw } from "vue";
  
  // Base URL de la API
  import URLBaseAPI from '../axiosAPIInstances.js';

  import HeaderOnigiri from './HeaderOnigiri.vue';
  import FooterComp from './FooterComp.vue';
  import LinkButton from './LinkButton.vue';
  import InputType from './InputType.vue';

  export default {
    components: {
      HeaderOnigiri,
      FooterComp,
      InputType,
      LinkButton
    },

    data() {
      return {
        // Guarda errores de validación
        errors: {},

        // Datos del formulario
        password: "",
        confPassword: "",
        zipCode: "",

        Persona: {
          Email: "",
          IdCard: "",
          Name1: "",
          Name2:"",
          Surname1: "",
          Surname2: "",
          Number: "",
          BirthdayDate: "",
          TypePerson: "Empleador",
          Status: "Inactivo"
        },

        // Datos para manejar la dirección
        Direccion: {
          selectedProvince: "",
          selectedCanton: "",
          selectedDistrit: "",
          otherSigns: "",
        },

        provincias: [],
        cantones: [],
        distritos: [],
      }
    },

    methods: {
      // Obtener lista de provincias
      GetProvince() {
        URLBaseAPI.get("/api/CountryDivision/Provinces")
        .then((response) => { this.provincias = response.data; });
      },

      // Obtener lista de cantones según la provincia seleccionada
      getCounties() {
        this.Direccion.selectedCanton = "";
        this.Direccion.selectedDistrit = "";
        this.cantones = [];
        this.distritos = [];

        URLBaseAPI.get("/api/CountryDivision/Counties", { params: {province: this.Direccion.selectedProvince}})
            .then((response) => { this.cantones = response.data; })
            .catch(console.error);
      },

      // Obtener lista de distritos según el cantón seleccionado
      getDistricts() {
        URLBaseAPI.get("/api/CountryDivision/Districts", { params: {province: this.Direccion.selectedProvince, county: this.Direccion.selectedCanton} })
            .then((response) => { this.distritos = response.data; })
            .catch(console.error);
      },

      // Obtener código postal según el distrito seleccionado
      getZipCode() {
        URLBaseAPI.get("/api/CountryDivision/ZipCode", { params: {province: this.Direccion.selectedProvince, county: this.Direccion.selectedCanton, district: this.Direccion.selectedDistrit} })
            .then((response) => { this.zipCode = response.data; })
            .catch(console.error);
      },

      // Validar los datos del formulario, agregar errores al objeto errors
      async validateForm() {
        this.errors = {};

        // Nombre y apellidos
        if (!this.Persona.Name1) this.errors.Name1 = "El primer nombre es obligatorio";
        if (!this.Persona.Surname1) this.errors.Surname1 = "El primer apellido es obligatorio";
        if (!this.Persona.Surname2) this.errors.Surname2 = "El segundo apellido es obligatorio";

        // Cédula
        if (!this.Persona.IdCard) this.errors.IdCard = "La cedula es obligatorio";
        else if (!/^\d{1}-\d{4}-\d{4}$/.test(this.Persona.IdCard)) this.errors.IdCard = "El formato debe ser #-####-####, solo valores numéricos";

        // Teléfono
        if (!this.Persona.Number) this.errors.Number = "El teléfono es obligatorio";
        else if (this.Persona.Number.length < 8) this.errors.Number = "El teléfono debe tener al menos 8 caracteres";
        else if (!/^\d{4}-\d{4}$/.test(this.Persona.Number)) this.errors.Number = "El formato debe ser ####-####, solo valores numéricos";

        // Fecha de nacimiento
        if (!this.Persona.BirthdayDate) this.errors.BirthdayDate = "La fecha de nacimiento es obligatoria";
        else {
          // Obtener fecha actual
          const today = new Date();
          // Convertir en objeto Date
          const birthDate = new Date(this.Persona.BirthdayDate);
          // Calcular la edad obteniendo los años
          var age = today.getFullYear() - birthDate.getFullYear();

          if (age < 18) this.errors.BirthdayDate = "Debe ser mayor de edad para registrarse";
        }

        // Dirección
        if (!this.Direccion.selectedProvince) this.errors.Provincia = "Debe seleccionar una provincia";
        if (!this.Direccion.selectedCanton) this.errors.Canton = "Debe seleccionar un cantón";
        if (!this.Direccion.selectedDistrit) this.errors.Distrito = "Debe seleccionar un distrito";
        if (!this.Direccion.otherSigns) this.errors.otherSigns = "Debe ingresar otras señas";

        // Correo y contraseñas
        if (!this.Persona.Email) this.errors.Email = "El correo es obligatorio";
        else if (!/\S+@\S+\.\S+/.test(this.Persona.Email)) this.errors.Email = "Correo inválido";
        else {
          // Esperar la verificación de correo
          try {
            const response = await URLBaseAPI.get("/api/PersonUser/emailCheck", { params: { email: this.Persona.Email } });
            if (response.data > 0) {
              this.errors.Email = "El correo ya está en uso";
            }
          } catch (error) {
            console.error(error);
          }
        }
        if (!this.password) this.errors.Password = "La contraseña es obligatoria";
        else if (this.password.length < 8) this.errors.Password = "La contraseña debe tener al menos 8 caracteres";
        else {
          if (!/[A-Z]/.test(this.password)) this.errors.Password = "La contraseña debe tener al menos una letra mayúscula";
          if (!/[a-z]/.test(this.password)) this.errors.Password = "La contraseña debe tener al menos una letra minúscula";
          if (!/[0-9]/.test(this.password)) this.errors.Password = "La contraseña debe tener al menos un número";
          if (!/[!@#$%^&*(),.?":{}|<>]/.test(this.password)) this.errors.Password = "La contraseña debe tener al menos un carácter especial";
        }
        if (this.password !== this.confPassword) this.errors.ConfPassword = "Las contraseñas no coinciden";

        // Si no hay errores, el formulario es válido
        return Object.keys(this.errors).length === 0;
      },

      // Enviar datos del formulario para guardar
      async saveRegisterData() {
        if (!(await this.validateForm())) {
          return;
        }

        const register = {
          personData: toRaw(this.Persona),
          password: this.password,
          otherSigns: this.Direccion.otherSigns,
          zipCode: this.zipCode.value
        }

        console.log(register);

        URLBaseAPI.post("/api/PersonUser/register", register)
            .then(response => { console.log("OK:", response.data);

              // Uso de alerta global para notificar un registro exitoso
              const alert = useGlobalAlert();
        
              alert.show("Registro exitoso. Por favor, active su cuenta mediante el correo de activación enviado a " + this.Persona.Email + " para poder iniciar sesión", "success");

              // Redireccionamiento que permite mantener la alerta
              this.$router.push("/");
            })
            .catch(error => {
                  if (error.response) console.log("Error del backend:", error.response.data);
                  else console.log("Error de red:", error.message);
              }
            );
        
        }
    },

    mounted() {
      this.GetProvince();
    }
  }
</script>

<style lang="scss" scoped>
  #container {
    background: #596D53;
    background: linear-gradient(357deg, rgba(89, 109, 83, 1) 0%, rgba(225, 245, 219, 1) 80%);
  }

  .form-group {
    margin-bottom: 20px;
  }

  .text-danger {
    font-weight: bold;
    font-size: medium;
  }

  #termsandlogin a {
    color: blue;
  }
</style>
