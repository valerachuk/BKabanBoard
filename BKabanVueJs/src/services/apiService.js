import axios from 'axios';
import router from '@/router';

const apiClient = axios.create({
    baseURL: "http://localhost:5000",
    // baseURL: "https://bkaban.azurewebsites.net",
    withCredentials: true,
    headers:{
        Accept: "application/json",
        'Content-Type': "application/json"
    }
});

function onError(err){
    if (err.response && err.response.status === 401){
        router.push({name: 'login'});
        return;
    }
    
    alert('API ERROR: ' + err.message);
    console.log('API ERROR: ' + err.message);
}

export default {
    getUserData(onLoad){
        apiClient.get('api/userData')
            .then(resp => onLoad(resp))
            .catch(err => onError(err));
    },

    createBoard(board, onLoad){
        apiClient.post('api/board', { id:board.id, name:board.name })
            .then(resp => onLoad(resp))
            .catch(err => onError(err));
    },
    getBoard(board, onLoad){
        apiClient.get(`/api/board/${board.id}`)
            .then(resp => onLoad(resp))
            .catch(err => onError(err));
    },
    updateBoardName(board){
        apiClient.put('/api/board', { name: board.name, id: board.id })
            .catch(err => onError(err));
    },
    updateBoardOrder(board, newIndex){
        apiClient.put('/api/board/reorder', {id: board.id, position: newIndex})
            .catch(err => onError(err));
    },
    deleteBoard(board){
        apiClient.delete(`/api/board/${board.id}`)
            .catch(err => onError(err));
    },

    createTask(task, columnId, onOk){
            apiClient.post('/api/task', {name: task.name, columnId})
            .then(resp => {
                task.id = resp.data.id;
                onOk(resp);
            })
            .catch(err => onError(err));
    },
    updateTask(task){
        apiClient.put('/api/task', task)
            .catch(err => onError(err));
    },
    updateTaskOrder(task, newIndex, newColumnId){
        apiClient.put('/api/task/reorder', {id: task.id, position: newIndex, newColumnId})
            .catch(err => onError(err));
    },
    deleteTask(task){
        apiClient.delete(`/api/task/${task.id}`)
            .catch(err => onError(err));
    },

    createColumn(column, boardId, okCb){
        apiClient.post('/api/column', {name: column.name, boardId})
            .then(resp => {
                column.id = resp.data.id;
                okCb(resp);
            })
            .catch(err => onError(err));
    },
    updateColumn(column){
        apiClient.put('/api/column', column)
            .catch(err => onError(err));
    },
    updateColumnOrder(column, newIndex){
        apiClient.put('/api/column/reorder', {id: column.id, position: newIndex})
            .catch(err => onError(err));
    },
    deleteColumn(column){
        apiClient.delete(`/api/column/${column.id}`)
            
            .catch(err => onError(err));
    },
    
    checkLogin(callback){
        apiClient.get('api/isLogged')
            .then(resp => callback(resp))
            .catch(err => onError(err));
    },
    login(user, okCb, errorCb){
        apiClient.post('api/login', user)
            .then(resp => okCb(resp))
            .catch(err => {
                errorCb(err);
                //onError(err);
            });
    },
    logout(okCb){
        apiClient.delete('api/logout')
            .then(resp => okCb(resp))
            .catch(err => onError(err));
    },
    register(user, okCb, errorCb){
        apiClient.post('api/register', user)
            .then(resp => okCb(resp))
            .catch(err => {
                // onError(err);
                errorCb(err);
            });
    }
    
}