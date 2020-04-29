<template>
<div class="form-wrapper">
    <form class="form">
        <img src="@/assets/kaban-logo.png" alt="logo" id="logo">
        <h1 class="form_title">{{mainAction}}</h1>
        <div class="form_group">
            <input type="text" class="form_input" placeholder=" " v-model="username" @input="isUsernameTipHilighted = false; isInvalidPwdOrUn = false; isInvalidRegister = false">
            <label class="form_label">Username</label>
            <div class="input-tip" :class="{hilighted: isUsernameTipHilighted}"  v-if="!this.isSignIn">
                Username length must be more than 2 and less than 20 characters
            </div>
        </div>
        <div class="form_group">
            <input type="password" class="form_input" placeholder=" " v-model="password" @input="isPasswordTipHilighted = false; isInvalidPwdOrUn = false">
            <label class="form_label">Password</label>
            <div class="input-tip" :class="{hilighted: isPasswordTipHilighted}" v-if="!this.isSignIn">
                Password length must be more than 6 and less than 100 characters
            </div>
            <div class="input-tip hilighted" v-if="this.isSignIn && isInvalidPwdOrUn">
                Invalid username or password
            </div>
            <div class="input-tip hilighted" v-if="!this.isSignIn && isInvalidRegister">
                Try another username
            </div>
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
                username: "",
                password: "",
                isUsernameTipHilighted: false,
                isPasswordTipHilighted: false,
                isInvalidPwdOrUn: false,
                isInvalidRegister: false
            };
        },
        computed: {
            mainAction: function () {
                return this.isSignIn ? "Sign in" : "Sign up";
            },
            optionAction: function () {
                return !this.isSignIn ? "Sign in" : "Sign up";
            },
            regPwdCorrect(){
                return this.password.length >= 6 && this.password.length <= 100;
            },
            regUsernameCorrect(){
                return this.username.length >= 2 && this.username.length <= 20;
            },
            btnDisabled: function () {
                return this.username.length === 0 || this.password.length === 0;
            },
        },
        methods: {
            processClick: function () {

                if (this.btnDisabled) {
                    return;
                }

                this.isUsernameTipHilighted = !this.regUsernameCorrect;
                this.isPasswordTipHilighted = !this.regPwdCorrect;

                if (!this.isSignIn && (!this.regUsernameCorrect || !this.regPwdCorrect)) {
                    return;
                }

                const user = {
                    username: this.username,
                    password: this.password
                };
                const goMain = () => this.$router.push({name: 'board'});
                if (this.isSignIn) {
                    apiClient.login(user, goMain, () => this.isInvalidPwdOrUn = true);
                }
                else{
                    apiClient.register(user, goMain, () => this.isInvalidRegister = true);
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
        letter-spacing: 1px;
    }

    .form_input,
    .form_button {
        letter-spacing: 1px;
        font-size: 16px;
    }

    .form_title {
        text-align: center;
        margin: 0;
        margin-bottom: 32px;
        margin-top: 16px;
        font-weight: normal;
        font-size: 32px;
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

    .input-tip{
        margin-top: 3px;
        color:rgb(126, 126, 126)
    }

    .input-tip.hilighted{
        color:rgb(214, 0, 0);
        /* font-weight: 600; */
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
        cursor: pointer;
    }

    .form_button.disabled {
        background-color: rgba(135, 190, 253, 0.7);
        cursor: default;
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