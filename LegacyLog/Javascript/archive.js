

const grid = document.getElementById("archiveGrid");
const search = document.getElementById("search");



search.addEventListener("input", e => render(e.target.value.toLowerCase()));
render();
