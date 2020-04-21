<template>
<div class="form-wrapper">
    <form class="form">
        <img src="@/assets/kaban-logo.png" alt="logo" id="logo">
        <h1 class="form_title">{{mainAction}}</h1>
        <div class="form_group">
            <input type="text" class="form_input" placeholder=" " v-model="username">
            <label class="form_label">Email</label>
        </div>
        <div class="form_group">
            <input type="password" class="form_input" placeholder=" " v-model="password">
            <label class="form_label">Password</label>
        </div>
        <div id="button-container">
            <button :class="{disabled: btnDisabled}" class="form_button"
                @click.prevent="processClick">{{mainAction}}</button>
            <p id="another-option" @click="isSignIn = !isSignIn">{{optionAction}}</p>
        </div>
    </form>
</div>
</template>

<script>
    import apiClient from '@/services/apiService.js';

    export default {
        data() {
            return {
                isSignIn: true,
                username: undefined,
                password: undefined,
            };
        },
        computed: {
            mainAction: function () {
                return this.isSignIn ? "Sign in" : "Sign up";
            },
            optionAction: function () {
                return !this.isSignIn ? "Sign in" : "Sign up";
            },
            btnDisabled: function () {
                return !(this.username && this.password && this.password.length >= 6 && this.username.length >= 2 &&
                    this.password.length <= 100 && this.username.length <= 20);
            },
        },
        methods: {
            processClick: function () {
                const user = {
                    username: this.username,
                    password: this.password
                };
                const goMain = () => this.$router.push({name: 'board'});
                const alertErr = (err) => alert(err.message);
                if (this.isSignIn) {
                    apiClient.login(user, goMain, alertErr);
                }
                else{
                    apiClient.register(user, goMain, alertErr);
                }
            }
        }
    }
</script>

<style scoped>
    .form-wrapper {
        display: flex;
        justify-content: center;
        align-items: center;
        height: 90vh;
    }

    #logo {
        width: 50%;
        margin: 0 auto;
        display: block;
    }

    .form {
        width: 300px;
        padding: 32px;
        border-radius: 10px;
        box-shadow: 0 4px 16px #cccccc;
        font-family: sans-serif;
        letter-spacing: 1px;
    }

    .form_input,
    .form_button {
        font-family: sans-serif;
        letter-spacing: 1px;
        font-size: 16px;
    }

    .form_title {
        text-align: center;
        margin: 0;
        margin-bottom: 32px;
        margin-top: 16px;
        font-weight: normal;
    }

    .form_group {
        position: relative;
        margin-bottom: 32px;
    }

    .form_label {
        position: absolute;
        top: 0;
        left: 0;
        z-index: -1;
        color: #9e9e9e;
        transition: 0.3s;
    }

    .form_input {
        width: 100%;
        padding: 0 0 10px 0;
        border: none;
        border-bottom: 1px solid #e0e0e0;
        background-color: transparent;
        outline: none;
        transition: 0.3s;
    }

    .form_input:focus {
        border-bottom: 1px solid #1a738a;
    }

    .form_button {
        padding: 10px 20px;
        border: none;
        border-radius: 5px;
        color: white;
        background-color: #0071f0;
        transition: 0.3s;
        font-weight: bolder;
        outline: none;
    }

    .form_button.disabled {
        background-color: rgba(135, 190, 253, 0.7);
    }

    .form_button:focus:not(.disabled),
    .form_button:hover:not(.disabled) {
        background-color: rgba(0, 113, 240, 0.7);
        cursor: pointer;
    }

    .form_input:focus~.form_label,
    .form_input:not(:placeholder-shown)~.form_label {
        top: -18px;
        font-size: 12px;
        color: #e0e0e0;
    }

    #another-option {
        text-decoration: none;
        color: #0071f0;
        font-weight: bolder;
        cursor: pointer;
    }

    #button-container {
        display: flex;
        justify-content: space-between;
        align-items: center;
    }
</style>