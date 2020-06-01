<template>
  <div id="app">
    <router-view />
  </div>
</template>

<style>
  * {
    user-select: none;
    margin: 0;
    padding: 0;
    font-size: 14px;
    color: #172b4d;
    font-family: -apple-system, BlinkMacSystemFont, Segoe UI, Roboto, Noto Sans, Ubuntu, Droid Sans, Helvetica Neue, sans-serif;
  }

  * {
    user-select: none;
    -khtml-user-select: none;
    -o-user-select: none;
    -moz-user-select: -moz-none;
    -webkit-user-select: none;
  }

  input, input:before, input:after {
    -webkit-user-select: initial;
    -khtml-user-select: initial;
    -moz-user-select: initial;
    -ms-user-select: initial;
    user-select: initial;
  }
</style>