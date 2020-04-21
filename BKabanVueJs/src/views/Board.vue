<template>
    <div>
        <div class="board-header">
            <div class="logout" @click="logout">
                Log-out
            </div>
            <input type="text" class="board-title" v-model="newBoardName" @change="renameBoard($event)"
                @keyup.enter="renameBoard($event)" v-if="isBoardAvailable">
            <img src="@/assets/kaban-logo.png" class="logo">
        </div>
        <div class="board" v-if="isBoardAvailable">
            <Column v-for="(column, index) of board.columns" :column="column" :key="column.id || index" :board="board"
                @open-modal-task="e => modalTaskObj = e" />
            <div class="column">
                <input type="text" class="new-column" placeholder="+ Add another column" v-model="newColumnName"
                    @keyup.enter="addColumn" @change="addColumn">
            </div>
        </div>
        <TaskEditModal v-if="modalTaskObj" :task="modalTaskObj" @close-modal-task="modalTaskObj = null" />
    </div>
</template>

<script>
    // import boardMock from '@/debug/boardMock.js';
    import Column from '@/components/Column.vue';
    import TaskEditModal from '@/components/TaskEditModal.vue';
    import apiClient from '@/services/apiService.js';

    export default {
        components: {
            Column,
            TaskEditModal
        },
        methods: {
            addColumn() {
                if (!this.newColumnName.trim()) {
                    return;
                }
                const newColumn = {
                    name: this.newColumnName,
                    tasks: [],
                };
                this.board.columns.push(newColumn);
                this.newColumnName = '';

                apiClient.createColumn(newColumn, () => this.$forceUpdate());
                //+++apply changes to API +++ DONE
            },
            renameBoard(e) {
                e.target.blur();

                if (!this.newBoardName.trim()) {
                    this.newBoardName = this.board.name;
                    return;
                }

                if (this.newBoardName === this.board.name) {
                    return;
                }

                this.board.name = this.newBoardName;

                apiClient.updateBoardName(this.board);
                //+++apply changes to API +++ DONE
            },
            logout(){
                apiClient.logout(() => this.$router.push({name: 'login'}))
            }
        },
        computed: {
            isBoardAvailable() {
                return Object.keys(this.board).length !== 0;
            },
        },
        watch:{
            board(){
                this.newBoardName = this.board.name;
            }
        },
        created() { //on api load DONE
            apiClient.getBoard(resp => this.board = resp.data);
        },
        data() {
            return {
                board: {},
                newColumnName: '',
                newBoardName: '',
                modalTaskObj: null
            }
        },
    }
</script>

<style>
    .board {
        height: calc(100vh - 70px);
        box-sizing: border-box;
        background-color: #0079BF;
        display: flex;
        align-items: flex-start;
        overflow-y: hidden;
        overflow-x: auto;
        padding-top: 4px;
    }

    .new-column {
        background-color: #fff;
        border: none;
        outline: none;
        margin: 8px;
        padding: 6px;
        border-radius: 3px;
        width: calc(100% - 16px);
        box-sizing: border-box;
    }

    .board-header {
        background-color: #0067A3;
        width: 100%;
        height: 70px;
        display: flex;
        align-items: center;
        justify-content: space-between;
    }

    .logout {
        background-color: rgba(255, 255, 255, 0.3);
        border-radius: 3px;
        font-weight: 600;
        font-size: 16px;
        color: white;
        padding: 4px 8px 6px;
        vertical-align: middle;
        margin-left: 8px;
        cursor: pointer;
    }

    .logout:hover {
        background-color: rgba(255, 255, 255, 0.4);
    }

    .logout:active {
        background-color: rgba(255, 255, 255, 0.5);
    }

    .logo {
        height: 100%;
        margin-right: 8px;
    }

    .board-title {
        outline: none;
        background: none;
        border: none;
        border-radius: 5px;
        cursor: pointer;
        width: 70%;
        height: 50px;
        text-align: center;
        color: #fff;
        font-weight: 600;
        font-size: 32px;
        padding-bottom: 5px;
    }

    .board-title:hover {
        background-color: rgba(0, 0, 0, 0.2);
    }

    .board-title:focus {
        cursor: text;
        background-color: rgba(0, 0, 0, 0.2);
    }
</style>