const api = '/api/todos';

async function fetchTodos() {
  const res = await fetch(api);
  const todos = await res.json();
  render(todos);
}

function render(todos) {
  const ul = document.getElementById('todos');
  ul.innerHTML = '';
  todos.forEach(t => {
    const li = document.createElement('li');
    li.className = 'todo-item';

    const cb = document.createElement('input');
    cb.type = 'checkbox';
    cb.checked = t.isDone;
    cb.addEventListener('change', async () => {
      await fetch(`${api}/${t.id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ title: t.title, isDone: cb.checked })
      });
      fetchTodos();
    });

    const span = document.createElement('span');
    span.textContent = t.title;
    if (t.isDone) span.classList.add('done');

    const del = document.createElement('button');
    del.textContent = 'Delete';
    del.addEventListener('click', async () => {
      await fetch(`${api}/${t.id}`, { method: 'DELETE' });
      fetchTodos();
    });

    li.appendChild(cb);
    li.appendChild(span);
    li.appendChild(del);
    ul.appendChild(li);
  });
}

document.getElementById('todo-form').addEventListener('submit', async (e) => {
  e.preventDefault();
  const input = document.getElementById('title');
  const title = input.value.trim();
  if (!title) return;
  await fetch(api, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ title, isDone: false })
  });
  input.value = '';
  fetchTodos();
});

fetchTodos();
