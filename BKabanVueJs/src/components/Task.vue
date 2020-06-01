<template>
    <div class="task" @click="openModal($event)">
        <div class="cross-container" @click="deleteTask" v-if="task.id">
            <img src="@/assets/icons/trash.svg" width="100%" class='cross-trash'/>
        </div>
        <div class="task-name">
            {{task.name}}
        </div>
        <div class="task-description" v-if="hasDescription">
            {{shortDescription}}
        </div>
    </div>
</template>

<script>
    import apiClient from '@/services/apiService.js';
    import { eventBus }  from '@/main.js';
    export default {
        props: {
            task: {
                type: Object,
                required: true
            },
            column: {
                type: Object,
                required: true
            }
        },
        computed: {
            hasDescription() {
                return this.task.description && this.task.description.trim();
            },
            shortDescription() {
                const maxLen = 80;
                return (this.hasDescription && this.task.description.length > maxLen) ? this.task.description.slice(0,
                    maxLen) + '...' : this.task.description;
            }
        },
        methods: {
            deleteTask(){
                apiClient.deleteTask(this.task);

                this.column.tasks = this.column.tasks.filter(task => this.task !== task);
            },
            openModal(e){

                if (!this.task.id ||
                 e.target.classList.contains('cross-trash') ||
                 e.target.classList.contains('cross-container')) {
                    return;
                }

                eventBus.$emit('open-modal-task', Object.assign({}, this.task, {colName: this.column.name}));
            }
        },
    }
</script>

<style>
    .task {
        background-color: #fff;
        padding: 6px 8px 2px;
        border-radius: 3px;
        margin: 0 8px 8px;
        box-shadow: 0 1px 0 rgba(9, 30, 66, .25);
        cursor: pointer;
        position: relative;
    }

    .task:hover {
        background-color: #F4F5F7;
    }

    .task-description {
        font-size: 12px;
        margin-bottom: 3px;
        color: #223b66;
    }

    .task-name {
        margin-bottom: 4px;
    }

    .task-description,
    .task-name {
        word-break: break-all;
    }

    .cross-container {
        width: 28px;
        height: 28px;
        position: absolute;
        right: 2px;
        top: 1.5px;
        padding: 6px;
        box-sizing: border-box;
        border-radius: 3px;
    }

    .cross-container>img {
        display: none;
    }

    .cross-container:hover{
        background-color: rgba(9,30,66,.04);;
    }

    .cross-container:active{
        background-color: rgba(9,30,66,.08);;
    }

    .task:hover>.cross-container>img {
        display: block;
    }

</style>