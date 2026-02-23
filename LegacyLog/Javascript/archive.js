const items = [
  { title: "Super Mario Bros.", console: "NES", year: 1985, region: "JP" },
  { title: "Sonic the Hedgehog", console: "Genesis", year: 1991, region: "US" },
  { title: "Chrono Trigger", console: "SNES", year: 1995, region: "JP" }
];

const grid = document.getElementById("archiveGrid");
const search = document.getElementById("search");

function render(filter = "") {
  grid.innerHTML = "";
  items
    .filter(i => JSON.stringify(i).toLowerCase().includes(filter))
    .forEach(i => {
      grid.innerHTML += `
        <div class="card">
          <h3>${i.title}</h3>
          <p class="muted">${i.console} • ${i.year} • ${i.region}</p>
        </div>`;
    });
}

search.addEventListener("input", e => render(e.target.value.toLowerCase()));
render();
