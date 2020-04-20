let vm = new Vue({
    el: "form",
    data: {
        isSignIn: true,
        email: undefined,
        password: undefined,
    },
    computed: {
        mainAction: function () {
            return this.isSignIn ? "Sign in" : "Sign up";
        },
        optionAction: function () {
            return !this.isSignIn ? "Sign in" : "Sign up";
        },
        btnDisabled: function () {
            return !(this.email && this.password && this.password.length >= 6 && this.email.length >= 6 && this.password.length <= 30 && this.email.length <= 30);
        },
        jsonCredentials: function () {
            return JSON.stringify({
                email: this.email,
                password: this.password
            });
        }
    },
    methods: {
        processClick: function () {
            if (!this.btnDisabled) {
                processApi(`api/${this.isSignIn ? "login" : "register"}`, "POST", this.jsonCredentials, () => document.location.reload(), alert);
            }
        }
    }
});