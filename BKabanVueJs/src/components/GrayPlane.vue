<template>
    <div class="gray-plane" @click.self="$emit('grayplane-click')">
        <slot></slot>
    </div>
</template>

<script>
    export default {
        
    }
</script>

<style scoped>
    .gray-plane{
        background-color: rgba(0, 0, 0, 0.5);
        width: 100vw;
        height:100vh;
        position: fixed;
        top: 0;
        z-index: 10;
    }
</style>