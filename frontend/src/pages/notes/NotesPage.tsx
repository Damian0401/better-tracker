import { useState, useEffect } from "react"
import { Api } from "@/libs/api"
import type { components } from "@/libs/api.schema.g"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Textarea } from "@/components/ui/textarea"
import { Separator } from "@/components/ui/separator"
import { ConfirmDialog } from "@/components/ConfirmDialog"
import { FormField } from "@/components/FormField"
import { ListWithSideSheetLayout } from "@/components/layout/ListWithSideSheetLayout"
import { toast } from "sonner"

type Note = components["schemas"]["ListNotesItemDto"]
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
  const [isLoadingMore, setIsLoadingMore] = useState(false)
  const [isModified, setIsModified] = useState(false)
  const [showDeleteDialog, setShowDeleteDialog] = useState(false)
  const [skip, setSkip] = useState(0)
  const [total, setTotal] = useState(0)

  const fetchNotes = async (skipParam: number = 0) => {
    const response = await Api.GET("/api/v1/notes", {
      params: { query: { Skip: skipParam, Count: 10 } },
    })
    if (!response.data) {
      toast.error("Failed to fetch notes")
      return
    }
    setTotal(response.data.total as number)
    if (skipParam === 0) {
      setNotes(response.data.items)
    } else {
      setNotes(prev => [...prev, ...response.data.items])
    }
    setSkip(skipParam + response.data.items.length)
  }

  const handleLoadMore = async () => {
    setIsLoadingMore(true)
    await fetchNotes(skip)
    setIsLoadingMore(false)
  }

  useEffect(() => {
    fetchNotes(0)
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
        if (!response.data) {
          toast.error("Failed to create note")
          return
        }
        toast.success("Note created successfully")
        const newNote: Note = {
          id: response.data.id,
          title: formData.title,
          content: formData.content,
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString(),
        }
        setNotes(prev => [newNote, ...prev])
        setTotal(prev => prev + 1)
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
        setNotes(prev => prev.map(n => 
          n.id === selectedNote.id ? { ...n, ...formData } : n
        ))
        setSelectedNote({ ...selectedNote, ...formData })
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
      setNotes(prev => prev.filter(n => n.id !== selectedNote.id))
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
    <>
      <ListWithSideSheetLayout
        sheetWidthClassName="max-w-3xl"
        sheetOpen={isCreating || !!selectedNote}
        onSheetOpenChange={(open) => {
          if (!open) {
            handleClose()
          }
        }}
        leftPanel={
          <>
            <div className="border-b p-4">
              <Button onClick={handleCreate} className="w-full">
                Create Note
              </Button>
            </div>
            <div className="min-h-0 flex-1 overflow-y-auto">
              {notes.map((note) => (
                <div
                  key={note.id}
                  onClick={() => handleSelectNote(note)}
                  className={`cursor-pointer border-b p-4 transition-colors hover:bg-muted ${
                    selectedNote?.id === note.id ? "bg-muted" : ""
                  }`}
                >
                  <h3 className="truncate font-medium">{note.title}</h3>
                  <p className="mt-1 text-sm text-muted-foreground">
                    {new Date(note.updatedAt).toLocaleDateString()}
                  </p>
                </div>
              ))}
              {notes.length < total && (
                <div className="p-4">
                  <Button
                    onClick={handleLoadMore}
                    variant="outline"
                    className="w-full"
                    disabled={isLoadingMore}
                  >
                    {isLoadingMore ? "Loading..." : "Load More"}
                  </Button>
                </div>
              )}
              {notes.length === 0 && (
                <div className="p-4 text-center text-muted-foreground">
                  No notes yet. Create your first note!
                </div>
              )}
            </div>
          </>
        }
        sheetContent={
          <div className="h-full overflow-y-auto p-6">
            <Card className="border-0 shadow-none">
              <CardHeader className="relative">
                <CardTitle>{isCreating ? "Create New Note" : "Edit Note"}</CardTitle>
                <Button
                  variant="ghost"
                  size="icon"
                  className="absolute right-4 top-4"
                  onClick={handleClose}
                >
                  <span className="text-xl">x</span>
                </Button>
              </CardHeader>
              <CardContent className="space-y-4">
                <FormField label="Title">
                  <Input
                    value={formData.title}
                    onChange={(e) => handleFormChange("title", e.target.value)}
                    placeholder="Enter note title"
                  />
                </FormField>
                <FormField label="Content">
                  <Textarea
                    value={formData.content}
                    onChange={(e) => handleFormChange("content", e.target.value)}
                    placeholder="Enter note content"
                    rows={10}
                  />
                </FormField>
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
          </div>
        }
      />

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
    </>
  )
}
