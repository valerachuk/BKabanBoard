<template>
    <div class="column">
        <div class="textarea-wrapper">
            <div class="cross-container" @click="deleteColumn" v-if="column.id">
                <img src="@/assets/icons/trash.svg" width="100%" />
            </div>
            <input class="name-edit" v-model="newColumnName" @change="renameColumn($event)"
                @keyup.enter="renameColumn($event)" />
        </div>
        <Task v-for="(task, index) of column.tasks" :key="task.id || index + task.name" :task="task" :column="column"/>
        <input class="add-task" placeholder="+ Add another task" v-model="newTaskName" @keyup.enter="addTask"
            @change="addTask" v-if="column.id" />
    </div>
</template>

<script>
    import apiClient from '@/services/apiService.js';
    import Task from '@/components/Task.vue';
    export default {
        components: {
            Task,
        },
        props: {
            column: {
                type: Object,
                required: true
            },
            board: {
                type: Object,
                required: true
            }
        },
        methods: {
            addTask() {
                if (!this.newTaskName.trim()) {
                    return;
                }
                const newTask = {
                    name: this.newTaskName.slice(0, 300),
                    description: '',
                };
                this.column.tasks.push(newTask);
                this.newTaskName = '';

                apiClient.createTask(newTask, this.column.id, () => this.$forceUpdate());
            },
            deleteColumn() {

                if (!confirm(`Delete ${this.column.name} ?`)) {
                    return;
                }

                apiClient.deleteColumn(this.column);
                
                this.board.columns = this.board.columns.filter(col => col !== this.column);
            },
            renameColumn(e) {
                e.target.blur();

                if (!this.newColumnName.trim() || !this.column.id) {
                    this.newColumnName = this.column.name;
                    return;
                }

                if (this.newColumnName === this.column.name) {
                    return;
                }

                this.newColumnName = this.newColumnName.slice(0, 100);

                this.column.name = this.newColumnName;

                apiClient.updateColumn(this.column);
            }
        },
        created() {
            this.newColumnName = this.column.name;
        },
        data() {
            return {
                newTaskName: '',
                newColumnName: ''
            }
        }
    }
</script>

<style>
    .column {
        background-color: #ebecf0;
        border-radius: 3px;
        min-width: 272px;
        width: 272px;
        margin: 4px;
        box-sizing: border-box;

        position: relative;
    }

    .column:first-child {
        margin-left: 8px;
    }

    .column-name {
        padding: 10px 8px;
        font-weight: 600;
        margin-bottom: 4px;
    }

    .textarea-wrapper:hover>.cross-container>img {
        display: block;
    }

    .textarea-wrapper:hover {
        cursor: pointer;
    }

    .name-edit {
        margin: 6px;
        padding: 4px 8px;
        font-weight: 600;
        width: 228px;
        margin: 6px 8px;
        box-sizing: border-box;
        cursor: pointer;
        border: 2px solid transparent;
    }

    .name-edit:focus {
        background-color: white;
        border: 2px solid #0079BF;
        border-radius: 3px;
        cursor: text;
    }

    .name-edit,
    .add-task {
        background: none;
        border-color: transparent;
        outline: none;
        resize: none;
    }

    .add-task {
        cursor: pointer;
        padding: 0 8px 8px;
        width: 100%;
        box-sizing: border-box;
    }

    .add-task:focus {
        cursor: text;
    }
</style>