class Race {
  constructor() {
    this.id = null;
    this.name = null;
    this.sizeId = null;
    this.baseSpeed = null;
    this.size = null;
  }

  static init(object) {
    let race = new Race();
    race.id = object.id;
    race.name = object.name;
    race.sizeId = object.sizeId;
    race.baseSpeed = object.baseSpeed;
    race.size = object.size;
    return race;
  }
}
