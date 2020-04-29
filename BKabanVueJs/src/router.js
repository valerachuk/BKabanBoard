import Vue from 'vue'
import VueRouter from 'vue-router'
import Login from '@/views/Login'
import apiClient from '@/services/apiService.js';

Vue.use(VueRouter)

const router = new VueRouter({
  // mode: 'history',
  // base: process.env.BASE_URL,
  routes: [
    {
      path: '/login',
      name: 'login',
      component: Login
    },
    {
      path: '*',
      name: 'board',
      component: () => import('./views/MainView')
    }
  ],
})

router.beforeEach((to, from, next) => {
  apiClient.checkLogin((resp) => {
    if (resp.data.isLogged) {
      if (to.name === 'board') {
        next();
      }
      else {
        next({name: 'board'});
      }
    }
    else{
      if (to.name === 'login') {
        next();
      } else {
        next({name: 'login'});
      }
    }
  });
});

export default router