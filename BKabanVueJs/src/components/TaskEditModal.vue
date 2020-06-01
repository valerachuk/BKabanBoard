<template>
    <GrayPlane>
        <div class="task-modal">
            <div class="cross-wrapper" @click="$emit('close-modal-task')">
                <img src="@/assets/icons/cross.svg">
            </div>
            <div class="name-wrapper">
                <img src="@/assets/icons/title.png" class="title-img">
                <input type="text" class="input-name" v-model="newName" @change="changeName($event)"
                    @keyup.enter="changeName($event)">
                <div class="in-column">In column: <span class="column-bold">{{task.colName}}</span></div>
            </div>
            <div class="description-wrapper">
                <img src="@/assets/icons/description.png" class="desc-img">
                <div class="description-text">Description:</div>
                <textarea class="desc-area" placeholder="Add task description..." v-model="newDescription"
                    @change="changeDescription($event)" @keyup.ctrl.enter="changeDescription($event)"/>
                </div>
            </div>
        </GrayPlane>
</template>

<script>
    import apiClient from '@/services/apiService.js';
    import GrayPlane from '@/components/GrayPlane.vue';
    export default {
        components:{
            GrayPlane
        },
        props: {
            task:{
                type: Object,
                required: true,
            }
        },
        methods:{
            changeName(e){
                e.target.blur();

                if (!this.newName.trim()) {
                    this.newName = this.task.name;
                    return;
                }

                if (this.newName === this.task.name) {
                    return;
                }

                this.newName = this.newName.slice(0, 300);

                this.task.name = this.newName;

                apiClient.updateTask({id: this.task.id, name:this.task.name});
            },
            changeDescription(e){
                if (this.newDescription.length > 1000) {
                    prompt('Description length is more than 1000 caracters! It will be trimmed, you can copy it from here', this.newDescription);
                }

                this.newDescription = this.newDescription.slice(0, 1000);
                
                e.target.blur();
                
                if (this.newDescription === this.task.description) {
                    return;
                }

                this.task.description = this.newDescription;

                apiClient.updateTask({id: this.task.id, description:this.task.description});
            }
        },
        created(){
            this.newName = this.task.name;
            this.newDescription = this.task.description;
        },
        data(){
            return{
                newDescription: '',
                newName: ''
            };
        }
    }
</script>

<style scoped>
    .task-modal{
        background-color: #F4F5F7;
        width: 575px;
        height: 400px;
        border-radius: 3px;
        position: absolute;
        top: 50%;
        left: 50%;
        margin-left: -287.5px;
        margin-top: -200px;
        padding: 20px;
    }

    .desc-img,
    .title-img{
        width: 30px;
        height: 30px;
    }

    .description-wrapper,
    .name-wrapper{
        display: flex;
        align-items: center;
        flex-wrap: wrap;
    }

    .name-wrapper{
        margin-bottom: 25px;
    }

    .input-name{
        font-size: 20px;
        font-weight: 600;
        background: none;
        outline: none;
        border: none;
        cursor: pointer;
        width: 500px;
        margin-left: 5px;
        padding: 0 6px 2px;
        border-radius: 3px;
        border: 2px solid transparent;
        box-sizing: border-box;
    }

    .desc-area:focus,
    .input-name:focus{
        border: 2px solid #0079BF;
        background-color: #fff !important;
        cursor: text;
    }

    .in-column{
        margin-left: 41px;
        margin-top: 5px;
    }

    .column-bold{
        font-weight: 600;
    }

    .description-text{
        font-size: 16px;
        font-weight: 600;
        margin-left: 12px;
    }

    .desc-area{
        cursor: pointer;
        border: 2px solid transparent;
        outline: none;
        resize: none;
        margin-left: 41px;
        width: 500px;
        height: 255px;
        padding: 8px 12px;
        border-radius: 3px;
        background-color: rgba(9,30,66,.04);
        margin-top: 5px;
    }

    .desc-area:hover{
        background-color: rgba(9,30,66,.08);
    }

    .cross-wrapper{
        right: 4px;
        top: 4px;
        width: 36px;
        height: 36px;
        position: absolute;
        border-radius: 50%;
        cursor: pointer;
    }

    .cross-wrapper:hover{
        background-color: rgba(9,30,66,.04);
    }

    .cross-wrapper:active{
        background-color: rgba(9,30,66,.08);
    }

    .cross-wrapper>img{
        height: 16px;
        width: 16px;
        margin: 10px;
    }

</style>