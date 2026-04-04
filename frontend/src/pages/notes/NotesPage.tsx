import { useState, useEffect } from "react"
import { Api } from "@/libs/api"
import type { components } from "@/libs/api.schema.g"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Textarea } from "@/components/ui/textarea"
import { Separator } from "@/components/ui/separator"
import { ConfirmDialog } from "@/components/ConfirmDialog"
import { toast } from "sonner"

type Note = components["schemas"]["Dto"]
type CreateNoteRequest = components["schemas"]["CreateNoteRequest"]
type UpdateNoteBody = components["schemas"]["UpdateNoteBody"]

export function NotesPage() {
  const [notes, setNotes] = useState<Note[]>([])
  const [selectedNote, setSelectedNote] = useState<Note | null>(null)
  const [isCreating, setIsCreating] = useState(false)
  const [formData, setFormData] = useState<CreateNoteRequest>({
    title: "",
    content: "",
  })
  const [isLoading, setIsLoading] = useState(false)
  const [isModified, setIsModified] = useState(false)
  const [showDeleteDialog, setShowDeleteDialog] = useState(false)

  const fetchNotes = async () => {
    const response = await Api.GET("/api/v1/notes")
    if (!response.data) {
      toast.error("Failed to fetch notes")
      return
    }
    setNotes(response.data.items)
  }

  useEffect(() => {
    fetchNotes()
  }, [])

  const handleSelectNote = async (note: Note) => {
    setIsCreating(false)
    setSelectedNote(note)
    setFormData({
      title: note.title,
      content: note.content,
    })
    setIsModified(false)
  }

  const handleCreate = () => {
    setIsCreating(true)
    setSelectedNote(null)
    setFormData({
      title: "",
      content: "",
    })
    setIsModified(false)
  }

  const handleSave = async () => {
    setIsLoading(true)
    try {
      if (isCreating) {
        const response = await Api.POST("/api/v1/notes", {
          body: formData,
        })
        if (response.error) {
          toast.error("Failed to create note")
          return
        }
        toast.success("Note created successfully")
        await fetchNotes()
        setIsCreating(false)
        setFormData({ title: "", content: "" })
        setIsModified(false)
      } else if (selectedNote) {
        const response = await Api.PUT("/api/v1/notes/{id}", {
          params: { path: { id: selectedNote.id } },
          body: formData as UpdateNoteBody,
        })
        if (response.error) {
          toast.error("Failed to update note")
          return
        }
        toast.success("Note updated successfully")
        await fetchNotes()
        const updatedNote = notes.find(n => n.id === selectedNote.id)
        if (updatedNote) {
          setSelectedNote({ ...updatedNote, ...formData })
        }
        setIsModified(false)
      }
    } finally {
      setIsLoading(false)
    }
  }

  const handleDeleteClick = () => {
    setShowDeleteDialog(true)
  }

  const handleDelete = async () => {
    if (!selectedNote) return
    
    setIsLoading(true)
    try {
      const response = await Api.DELETE("/api/v1/notes/{id}", {
        params: { path: { id: selectedNote.id } },
      })
      if (response.error) {
        toast.error("Failed to delete note")
        return
      }
      toast.success("Note deleted successfully")
      await fetchNotes()
      setSelectedNote(null)
      setFormData({ title: "", content: "" })
      setIsModified(false)
    } finally {
      setIsLoading(false)
    }
  }

  const handleCancel = () => {
    setIsCreating(false)
    setSelectedNote(null)
    setFormData({ title: "", content: "" })
    setIsModified(false)
  }

  const handleClose = () => {
    setIsCreating(false)
    setSelectedNote(null)
    setFormData({ title: "", content: "" })
    setIsModified(false)
  }

  const handleFormChange = (field: keyof CreateNoteRequest, value: string) => {
    setFormData({ ...formData, [field]: value })
    if (!isCreating && selectedNote) {
      // Check if the value is different from the original
      const hasChanged = 
        (field === "title" && value !== selectedNote.title) ||
        (field === "content" && value !== selectedNote.content) ||
        (field === "title" && formData.content !== selectedNote.content) ||
        (field === "content" && formData.title !== selectedNote.title)
      setIsModified(hasChanged)
    }
  }

  return (
    <div className="flex h-full">
      {/* Notes List */}
      <div className="w-80 border-r">
        <div className="border-b p-4">
          <Button onClick={handleCreate} className="w-full">
            Create Note
          </Button>
        </div>
        <div className="overflow-auto">
          {notes.map((note) => (
            <div
              key={note.id}
              onClick={() => handleSelectNote(note)}
              className={`cursor-pointer border-b p-4 transition-colors hover:bg-muted ${
                selectedNote?.id === note.id ? "bg-muted" : ""
              }`}
            >
              <h3 className="font-medium truncate">{note.title}</h3>
              <p className="text-sm text-muted-foreground mt-1">
                {new Date(note.updatedAt).toLocaleDateString()}
              </p>
            </div>
          ))}
          {notes.length === 0 && (
            <div className="p-4 text-center text-muted-foreground">
              No notes yet. Create your first note!
            </div>
          )}
        </div>
      </div>

      {/* Note Details */}
      <div className="flex-1 p-6">
        {(selectedNote || isCreating) ? (
          <Card>
            <CardHeader className="relative">
              <CardTitle>{isCreating ? "Create New Note" : "Edit Note"}</CardTitle>
              <Button
                variant="ghost"
                size="icon"
                className="absolute right-4 top-4"
                onClick={handleClose}
              >
                <span className="text-xl">×</span>
              </Button>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="space-y-2">
                <label className="text-sm font-medium">Title</label>
                <Input
                  value={formData.title}
                  onChange={(e) => handleFormChange("title", e.target.value)}
                  placeholder="Enter note title"
                />
              </div>
              <div className="space-y-2">
                <label className="text-sm font-medium">Content</label>
                <Textarea
                  value={formData.content}
                  onChange={(e) => handleFormChange("content", e.target.value)}
                  placeholder="Enter note content"
                  rows={10}
                />
              </div>
              <Separator />
              <div className="flex gap-2">
                {isCreating ? (
                  <>
                    <Button onClick={handleSave} disabled={isLoading}>
                      {isLoading ? "Saving..." : "Save"}
                    </Button>
                    <Button onClick={handleCancel} variant="outline" disabled={isLoading}>
                      Cancel
                    </Button>
                  </>
                ) : (
                  <>
                    <Button onClick={handleSave} disabled={isLoading || !isModified}>
                      {isLoading ? "Saving..." : "Save"}
                    </Button>
                    <Button onClick={handleDeleteClick} variant="destructive" disabled={isLoading}>
                      Delete
                    </Button>
                  </>
                )}
              </div>
            </CardContent>
          </Card>
        ) : (
          <div className="flex h-full items-center justify-center text-muted-foreground">
            Select a note to view details or create a new one
          </div>
        )}
      </div>

      <ConfirmDialog
        open={showDeleteDialog}
        onOpenChange={setShowDeleteDialog}
        onConfirm={handleDelete}
        title="Delete Note"
        description="Are you sure you want to delete this note? This action cannot be undone."
        confirmText="Delete"
        cancelText="Cancel"
        variant="destructive"
      />
    </div>
  )
}
