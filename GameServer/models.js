let User = mongoose.model('User', {
    id: Number,
    username: String,
    password: String,
    email: String,
    date_reg: Number
});

let Character = mongoose.model('Character', {
    id: Number,
    coins: Number
});

let Item = mongoose.model('Item', {
    id: Number,
    name: String,
    picture: String,
    price: Number,
    is_premium: Boolean,
    type: String
});

let Character_item = mongoose.model('Character_item', {
    character_id: Number,
    item_id: Number,
    picture: String,
    is_on: Boolean
});

let Counter = mongoose.model('Counter', {
   user_id: Number,
    item_id: Number
});