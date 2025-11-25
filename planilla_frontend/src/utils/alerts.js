
// Pinia nos permite crear un tipo de alerta global
import { defineStore } from 'pinia'


export const useGlobalAlert = defineStore('globalAlert', {
  state: () => ({ message: null, type: null }),
  actions: {
    show(message, type = "success") {
      this.message = message
      this.type = type
    },
    
    clear() {
      this.message = null
      this.type = null
    }
  }
})
