import axios from "axios";

const apiClient = axios.create({
    // baseURL: "http://localhost:58813/",
    withCredentials: true,
    headers:{
        Accept: "application/json",
        'Content-Type': "application/json"
    }
});

function onError(err){
    console.log('API ERROR: ' + err.message);
}

export default {
    getBoard(e){
        apiClient.get('/api/board')
            .then(resp => e(resp))
            .catch(err => onError(err));
    },
    updateBoardName(board){
        apiClient.put('/api/board/rename', { name: board.name })
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
    deleteTask(task){
        apiClient.delete(`/api/task/${task.id}`)
            .catch(err => onError(err));
    },

    createColumn(column, okCb){
        apiClient.post('/api/column', {name: column.name})
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
                onError(err);
            });
    },
    logout(okCb, errorCb){
        apiClient.delete('api/logout')
            .then(resp => okCb(resp))
            .catch(err => {
                errorCb(err);
                onError(err);
            });
    },
    register(user, okCb, errorCb){
        apiClient.post('api/register', user)
            .then(resp => okCb(resp))
            .catch(err => {
                errorCb(err);
                onError(err);
            });
    }
    
}