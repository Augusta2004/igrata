const mongoose = require('mongoose');

let userSchema = mongoose.Schema({
    user_id: Number,
    username: String,
    password: String,
    mail: String,
    date_reg: Number
});

const User = mongoose.model('User', userSchema);
module.exports = User;