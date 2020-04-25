<template>
    <div class="app-wrapper">
        <div class="boards-list">
            <div class="list-title-wrapper">
                <div class="list-title">Your boards:</div>
            </div>
            <div class="board-list-wrapper" v-if="isDataAvailable">
                <div v-for="(board, index) of userData.boards" :key="board.id || index + board.name"
                    class="name-container" @click="openBoard($event, board)">
                    <div class="cross-container" v-if="board.id" @click="deleteBoard(board)">
                        <img src="@/assets/icons/trash.svg" width="100%" class='cross-trash' />
                    </div>
                    <div class="board-name">
                        {{board.name}}
                    </div>
                </div>
                <input type="text" placeholder="+ Add another board" class="add-new-board" v-model="newBoardName" @change="createBoard" @keyup.enter="createBoard">
            </div>
        </div>
        <Board class="board-wrapper" :board="currentBoard"/>
        <TaskEditModal v-if="modalTaskObj" :task="modalTaskObj" @close-modal-task="modalTaskObj = null" />
    </div>
</template>

<script>
    import apiClient from '@/services/apiService.js';
    import Board from '@/views/Board.vue'; 
    import TaskEditModal from '@/components/TaskEditModal.vue';
    import {eventBus} from '@/main.js';

    export default {
        components: {
            Board,
            TaskEditModal
        },
        data() {
            return {
                userData: {},
                modalTaskObj: null,
                currentBoard: {},
                newBoardName: ''
            };
        },
        created(){
            eventBus.$on('open-modal-task', task => this.modalTaskObj = task);
            eventBus.$on('rename-board', board => this.userData.boards.forEach(elem => {
                if (elem.id === board.id) {
                    elem.name = board.name;
                }
            }));
            apiClient.getUserData(resp => this.userData = resp.data)
        },
        computed: {
            isDataAvailable() {
                return Object.keys(this.userData).length !== 0;
            }
        },
        methods: {
            openBoard(e, board){
                if (!board.id ||
                 e.target.classList.contains('cross-trash') ||
                 e.target.classList.contains('cross-container') ||
                 board.id === this.currentBoard.id) {
                    return;
                }

                this.modalTaskObj = null;

                apiClient.getBoard(board, resp => this.currentBoard = resp.data);
            },
            deleteBoard(board){
                const delAns = prompt(`Type "delete" to delete "${board.name}"`);
                if (delAns && delAns.toLowerCase() === 'delete') {
                    if (board.id === this.currentBoard.id) {
                        this.currentBoard = {};
                    }
                    this.userData.boards = this.userData.boards.filter(b => b !== board);
                    
                    apiClient.deleteBoard(board);   
                }
            },
            createBoard(){
                if (!this.newBoardName.trim()) {
                    return;
                }

                const newBoardCard = {name: this.newBoardName.slice(0, 100)};
                this.newBoardName = '';

                this.userData.boards.push(newBoardCard);

                apiClient.createBoard(newBoardCard, resp => {
                    newBoardCard.id = resp.data.id;
                    this.$forceUpdate();
                });
            }
        }
    }
</script>

<style scoped>
    .app-wrapper{
        display: flex;
    }

    .boards-list{
        border-right: 4px solid rgb(0, 61, 97);
        background-color: #0079BF;
    }

    .list-title-wrapper{
        background-color: #0067A3;
        height: 70px;
        display: flex;
        align-items: center;
    }
    
    .list-title{
        font-size: 24px;
        font-weight: 600;
        color: white;
        margin-left: 20px;
    }

    .board-list-wrapper{
        background-color: #0079BF;
        padding: 8px;
        color: white;
    }

    .name-container{
        width: 250px;
        position: relative;
        background-color: rgb(0, 61, 97);
        padding: 6px 8px;
        border-radius: 3px;
        margin: 0 0 8px;
        cursor: pointer;
        box-sizing: border-box;
    }

    .name-container:hover{
        background-color: rgb(0, 43, 68);
    }

    .board-name{
        color: white;
        font-size: 16px;
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
        background-color: rgb(0, 85, 134);
    }

    .cross-container:active{
        background-color: rgb(0, 109, 172);
    }

    .name-container:hover>.cross-container>img {
        display: block;
    }

    .board-wrapper{
        flex-grow: 1;
    }

    .add-new-board{
        width: 250px;
        outline: none;
        background: none;
        border: none;
        color: white;
        font-size: 16px;
        cursor: pointer;
    }

    .add-new-board:focus{
        cursor: text;
    }

    .add-new-board::placeholder{
        color: #dfdfdf;
        font-size: 16px;
    }

</style>